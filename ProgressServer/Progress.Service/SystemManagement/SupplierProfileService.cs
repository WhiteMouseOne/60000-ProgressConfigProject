using Microsoft.EntityFrameworkCore;
using Progress.IService;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.SystemManagement;
using Progress.Repository;

namespace Progress.Service.SystemManagement
{
    public class SupplierProfileService : ISupplierProfileService
    {
        private readonly ProgressDbContext _db;
        private readonly ICurrentUser _current;

        public SupplierProfileService(ProgressDbContext db, ICurrentUser current)
        {
            _db = db;
            _current = current;
        }

        public async Task<SupplierProfileDto?> GetAsync(CancellationToken ct = default)
        {
            if (!_current.Roles.Contains("Supplier") || _current.SupplierId is not int sid)
                return null;

            var user = await _db.Users!.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == _current.UserId && u.SupplierId == sid, ct);
            if (user == null) return null;

            var org = await _db.Suppliers!.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == sid, ct);
            if (org == null) return null;

            return new SupplierProfileDto
            {
                Org = new SupplierOrgDto
                {
                    Id = org.Id,
                    SupplierNumber = org.SupplierNumber,
                    Name = org.Name
                },
                Account = new SupplierAccountDto
                {
                    EmployeeNumber = user.EmployeeNumber,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    HeadPortrait = user.HeadPortrait
                }
            };
        }

        public async Task<(bool ok, string message)> UpdateAsync(SupplierProfileUpdateDto dto, CancellationToken ct = default)
        {
            if (!_current.Roles.Contains("Supplier") || _current.SupplierId is not int sid)
                return (false, "仅供应商账号可维护资料");

            var user = await _db.Users!.FirstOrDefaultAsync(u => u.Id == _current.UserId && u.SupplierId == sid, ct);
            if (user == null) return (false, "用户不存在");

            if (dto.UserName != null) user.UserName = dto.UserName.Trim();
            if (dto.PhoneNumber != null) user.PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber) ? null : dto.PhoneNumber.Trim();
            if (dto.Email != null) user.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
            if (dto.HeadPortrait != null) user.HeadPortrait = string.IsNullOrWhiteSpace(dto.HeadPortrait) ? null : dto.HeadPortrait.Trim();

            user.UpdateTime = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            return (true, "已保存");
        }
    }
}
