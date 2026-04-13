using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Progress.Common;
using Progress.IService;
using Progress.IService.Business;
using Progress.Model;
using Progress.Model.Dto.Order;
using Progress.Model.Entitys;
using Progress.Repository;

namespace Progress.Service.Business
{
    public class RepairService : IRepairService
    {
        private readonly ProgressDbContext _db;
        private readonly IDataScopeService _scope;
        private readonly ICurrentUser _user;
        private readonly IEmailSender _email;
        private readonly EmailOptions _mailOpt;
        private readonly ILogger<RepairService> _log;

        public RepairService(
            ProgressDbContext db,
            IDataScopeService scope,
            ICurrentUser user,
            IEmailSender email,
            IOptions<EmailOptions> mailOpt,
            ILogger<RepairService> log)
        {
            _db = db;
            _scope = scope;
            _user = user;
            _email = email;
            _mailOpt = mailOpt.Value;
            _log = log;
        }

        public async Task<(bool ok, string message)> CreateAsync(RepairCreateRequest req, CancellationToken ct = default)
        {
            if (!await _scope.CanAccessLineAsync(req.OrderLineId, ct))
                return (false, "无权操作该行");
            var line = await _db.WorkpieceOrderLines!.Include(x => x.Supplier)
                .FirstOrDefaultAsync(x => x.Id == req.OrderLineId, ct);
            if (line == null) return (false, "订单行不存在");

            var now = DateTime.UtcNow;
            line.RepairStatus = OrderRepairStatus.PendingSupplier;
            line.RepairCreatedAt = now;
            line.RepairStartedAt = null;
            line.RepairShippedAt = null;
            line.VendorUpdatedAt = now;
            line.UpdateTime = now;

            var rec = new RepairRecord
            {
                OrderLineId = req.OrderLineId,
                Description = req.Description,
                CreatedByUserId = _user.UserId,
                CreatedAt = DateTime.UtcNow
            };
            _db.RepairRecords!.Add(rec);
            await _db.SaveChangesAsync(ct);

            var recipients = _mailOpt.RepairRecipients;
            var subject = $"[加工件返修] PO:{line.PoNumber} 行号:{line.LineNo} {line.PartName}";
            var body = $"创建人: {_user.UserName} ({_user.EmployeeNumber})\n供应商: {line.Supplier?.Name}\n说明: {req.Description}\n行 Id: {line.Id}";
            var sent = await _email.SendAsync(subject, body, recipients, ct);
            rec.EmailSentAt = sent ? DateTime.UtcNow : null;
            await _db.SaveChangesAsync(ct);

            if (!sent && !string.IsNullOrEmpty(recipients))
                _log.LogWarning("Repair record {Id} saved but email not sent", rec.Id);

            return (true, sent ? "返修已记录并已发送邮件" : "返修已记录（邮件未发送或无需发送）");
        }
    }
}
