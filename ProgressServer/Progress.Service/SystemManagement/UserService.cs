using Microsoft.EntityFrameworkCore;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.SystemManagement;
using Progress.Model.Entitys;
using Progress.Repository;

namespace Progress.Service.SystemManagement
{
    public class UserService : IUserService
    {
        private const int ResetPasswordEnableCode = 101;
        private readonly ProgressDbContext _db;

        public UserService(ProgressDbContext db)
        {
            _db = db;
        }

        public async Task<(int total, List<UserListRow> dataList)> GetUserDataAsync(UserGetRequest req, CancellationToken ct = default)
        {
            var q = _db.Users!.AsNoTracking().Where(u => u.IsDeleted == 0);
            if (!string.IsNullOrWhiteSpace(req.employeeNumber))
                q = q.Where(u => u.EmployeeNumber.Contains(req.employeeNumber));
            if (!string.IsNullOrWhiteSpace(req.userName))
                q = q.Where(u => u.UserName.Contains(req.userName));
            if (req.enable is int en)
                q = q.Where(u => u.Enable == en);

            var total = await q.CountAsync(ct);
            var page = Math.Max(1, req.page);
            var size = Math.Max(1, req.size);
            var list = await q
                .OrderBy(u => u.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(u => new UserListRow
                {
                    id = u.Id,
                    employeeNumber = u.EmployeeNumber,
                    userName = u.UserName,
                    phoneNumber = u.PhoneNumber,
                    email = u.Email,
                    headPortrait = u.HeadPortrait,
                    enable = u.Enable,
                    createBy = u.CreateBy,
                    createTime = u.CreateTime,
                    updateBy = u.UpdateBy,
                    updateTime = u.UpdateTime,
                    isDeleted = u.IsDeleted
                })
                .ToListAsync(ct);

            return (total, list);
        }

        public async Task<(bool ok, string message, PersonInfoDto? data)> GetPersonInfoAsync(string employeeNumber, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(employeeNumber))
                return (false, "employeeNumber required", null);

            var u = await _db.Users!.AsNoTracking()
                .FirstOrDefaultAsync(x => x.EmployeeNumber == employeeNumber && x.IsDeleted == 0, ct);
            if (u == null)
                return (false, "用户不存在", null);

            var roleName = await (
                from ur in _db.UserRoles!
                join r in _db.Roles! on ur.RoleId equals r.Id
                where ur.UserId == u.Id
                orderby r.RoleSort
                select r.RoleName
            ).FirstOrDefaultAsync(ct) ?? "";

            var dto = new PersonInfoDto
            {
                id = u.Id,
                employeeNumber = u.EmployeeNumber,
                userName = u.UserName,
                phoneNumber = u.PhoneNumber,
                email = u.Email,
                headPortrait = u.HeadPortrait,
                roleName = roleName,
                password = ""
            };
            return (true, "Success!", dto);
        }

        public async Task<(bool ok, string message)> AddUserAsync(UserAddDto dto, CancellationToken ct = default)
        {
            var emp = dto.employeeNumber?.Trim() ?? "";
            if (string.IsNullOrEmpty(emp))
                return (false, "工号不能为空");
            if (await _db.Users!.AnyAsync(u => u.EmployeeNumber == emp && u.IsDeleted == 0, ct))
                return (false, "工号已存在");

            var enable = ParseEnableString(dto.enable);
            var now = DateTime.UtcNow;
            var user = new Users
            {
                EmployeeNumber = emp,
                UserName = dto.userName?.Trim() ?? "",
                Password = dto.password,
                PhoneNumber = dto.phoneNumber,
                Email = string.IsNullOrWhiteSpace(dto.email) ? null : dto.email.Trim(),
                Enable = enable,
                CreateBy = dto.createBy,
                CreateTime = now,
                IsDeleted = 0,
                IsSupplierAccount = 0,
                SupplierId = null
            };
            _db.Users!.Add(user);
            await _db.SaveChangesAsync(ct);

            await SyncUserRoleAsync(user.Id, dto.roleIds, ct);
            return (true, "新增成功");
        }

        public async Task<(bool ok, string message)> UpdateUserAsync(UserUpdateDto dto, CancellationToken ct = default)
        {
            var user = await _db.Users!.FirstOrDefaultAsync(u => u.Id == dto.id && u.IsDeleted == 0, ct);
            if (user == null)
                return (false, "用户不存在");

            var emp = dto.employeeNumber?.Trim() ?? "";
            if (await _db.Users!.AnyAsync(u => u.EmployeeNumber == emp && u.Id != dto.id && u.IsDeleted == 0, ct))
                return (false, "工号已存在");

            user.EmployeeNumber = emp;
            user.UserName = dto.userName?.Trim() ?? "";
            user.Password = dto.password;
            user.PhoneNumber = dto.phoneNumber;
            user.Email = string.IsNullOrWhiteSpace(dto.email) ? null : dto.email.Trim();
            user.Enable = ParseEnableString(dto.enable);
            user.UpdateBy = dto.updateBy;
            user.UpdateTime = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            await SyncUserRoleAsync(user.Id, dto.roleIds, ct);
            return (true, "修改成功");
        }

        public async Task<(bool ok, string message)> DeleteUserAsync(int id, CancellationToken ct = default)
        {
            if (id == 1)
                return (false, "请勿删除超级管理员用户。");

            var user = await _db.Users!.FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted == 0, ct);
            if (user == null)
                return (false, "用户不存在");

            user.IsDeleted = 1;
            user.UpdateTime = DateTime.UtcNow;
            await _db.UserRoles!.Where(ur => ur.UserId == id).ExecuteDeleteAsync(ct);
            await _db.SaveChangesAsync(ct);
            return (true, "删除成功");
        }

        public async Task<(bool ok, string message)> BatchDeleteUsersAsync(IReadOnlyList<int> ids, CancellationToken ct = default)
        {
            if (ids == null || ids.Count == 0)
                return (false, "请选择要删除的用户");

            var set = ids.Where(i => i != 1).Distinct().ToList();
            foreach (var id in set)
            {
                var user = await _db.Users!.FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted == 0, ct);
                if (user == null) continue;
                user.IsDeleted = 1;
                user.UpdateTime = DateTime.UtcNow;
                await _db.UserRoles!.Where(ur => ur.UserId == id).ExecuteDeleteAsync(ct);
            }

            await _db.SaveChangesAsync(ct);
            return (true, "批量删除成功");
        }

        public async Task<(bool ok, string message)> ResetPwdOrStatusAsync(ResetPwdOrStatusDto dto, CancellationToken ct = default)
        {
            var user = await _db.Users!.FirstOrDefaultAsync(u => u.Id == dto.id && u.IsDeleted == 0, ct);
            if (user == null)
                return (false, "用户不存在");

            if (dto.enable == ResetPasswordEnableCode)
            {
                if (string.IsNullOrEmpty(dto.password))
                    return (false, "密码不能为空");
                user.Password = dto.password;
            }
            else
            {
                user.Enable = dto.enable;
            }

            user.UpdateTime = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return (true, "操作成功");
        }

        public async Task<(bool ok, string message)> UpdateHeadPortraitPathAsync(string employeeNumber, string? relativePath, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(employeeNumber))
                return (false, "employeeNumber required");

            var user = await _db.Users!.FirstOrDefaultAsync(u => u.EmployeeNumber == employeeNumber && u.IsDeleted == 0, ct);
            if (user == null)
                return (false, "用户不存在");

            user.HeadPortrait = relativePath;
            user.UpdateTime = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return (true, "上传成功");
        }

        public async Task<(bool ok, string message)> DeleteHeadPortraitAsync(string employeeNumber, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(employeeNumber))
                return (false, "employeeNumber required");

            var user = await _db.Users!.FirstOrDefaultAsync(u => u.EmployeeNumber == employeeNumber && u.IsDeleted == 0, ct);
            if (user == null)
                return (false, "用户不存在");

            user.HeadPortrait = null;
            user.UpdateTime = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return (true, "ok");
        }

        public async Task<(bool ok, string message)> UpdatePersonalInfoAsync(UpdatePersonalInfoDto dto, CancellationToken ct = default)
        {
            var user = await _db.Users!.FirstOrDefaultAsync(u => u.Id == dto.id && u.IsDeleted == 0, ct);
            if (user == null)
                return (false, "用户不存在");

            if (!string.IsNullOrEmpty(dto.newPassword))
            {
                if (string.IsNullOrEmpty(dto.oldPassword) || user.Password != dto.oldPassword)
                    return (false, "The old password is incorrect");
                user.Password = dto.newPassword!;
            }

            if (dto.userName != null)
                user.UserName = dto.userName;
            if (dto.phoneNumber != null)
                user.PhoneNumber = dto.phoneNumber;
            if (dto.email != null)
                user.Email = string.IsNullOrWhiteSpace(dto.email) ? null : dto.email.Trim();

            user.UpdateTime = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);

            if (!string.IsNullOrWhiteSpace(dto.roleName))
            {
                var role = await _db.Roles!.FirstOrDefaultAsync(r => r.RoleName == dto.roleName.Trim(), ct);
                if (role != null)
                    await SyncUserRoleAsync(user.Id, role.Id, ct);
            }

            return (true, "Operate Success!");
        }

        private static int ParseEnableString(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 1;
            var t = s.Trim();
            if (t == "1" || t.Equals("true", StringComparison.OrdinalIgnoreCase)) return 1;
            return 0;
        }

        private async Task SyncUserRoleAsync(int userId, int roleId, CancellationToken ct)
        {
            await _db.UserRoles!.Where(ur => ur.UserId == userId).ExecuteDeleteAsync(ct);
            _db.UserRoles!.Add(new UserRole { UserId = userId, RoleId = roleId });
            await _db.SaveChangesAsync(ct);
        }
    }
}
