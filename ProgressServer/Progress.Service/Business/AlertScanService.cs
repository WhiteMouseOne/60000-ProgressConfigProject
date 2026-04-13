using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Progress.Common;
using Progress.IService.Business;
using Progress.Model;
using Progress.Model.Entitys;
using Progress.Repository;

namespace Progress.Service.Business
{
    public class AlertScanService : IAlertScanService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly EmailOptions _mailOpt;
        private readonly ILogger<AlertScanService> _log;

        public AlertScanService(
            IServiceScopeFactory scopeFactory,
            IOptions<EmailOptions> mailOpt,
            ILogger<AlertScanService> log)
        {
            _scopeFactory = scopeFactory;
            _mailOpt = mailOpt.Value;
            _log = log;
        }

        public async Task RunOnceAsync(CancellationToken ct = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ProgressDbContext>();
            var email = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            var setting = await db.AlertSettings!.AsNoTracking().FirstOrDefaultAsync(ct);
            if (setting is not { Enabled: true }) return;

            var today = DateTime.UtcNow.Date;
            var dayKey = today.ToString("yyyy-MM-dd");

            var candidates = await db.WorkpieceOrderLines!.AsNoTracking()
                .Where(x => x.ShippingStatus != OrderShippingStatus.Shipped && x.RequiredDeliveryDate != null)
                .Where(x => today >= x.RequiredDeliveryDate!.Value.Date.AddDays(-setting.LeadDays))
                .Select(x => new { x.Id, x.PoNumber, x.LineNo, x.PartName, x.RequiredDeliveryDate })
                .ToListAsync(ct);

            foreach (var c in candidates)
            {
                var exists = await db.AlertNotificationLogs!.AnyAsync(
                    l => l.OrderLineId == c.Id && l.AlertDay == dayKey, ct);
                if (exists) continue;

                var subject = $"[加工件预警] PO:{c.PoNumber} 行:{c.LineNo} 交期:{c.RequiredDeliveryDate:yyyy-MM-dd}";
                var body = $"订单行 Id: {c.Id}\n加工件: {c.PartName}\n请在系统中关注进度。";
                var ok = await email.SendAsync(subject, body, _mailOpt.AlertRecipients, ct);
                if (ok)
                {
                    db.AlertNotificationLogs.Add(new AlertNotificationLog
                    {
                        OrderLineId = c.Id,
                        AlertDay = dayKey,
                        SentAt = DateTime.UtcNow
                    });
                    await db.SaveChangesAsync(ct);
                }
                else
                    _log.LogWarning("Alert email not sent for line {Id}; will retry next scan", c.Id);
            }
        }
    }
}
