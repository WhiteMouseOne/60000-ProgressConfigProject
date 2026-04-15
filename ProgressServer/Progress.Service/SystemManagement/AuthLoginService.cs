using Microsoft.EntityFrameworkCore;
using Progress.IService;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.Menu;
using Progress.Repository;

namespace Progress.Service.SystemManagement
{
    public class AuthLoginService : IAuthLoginService
    {
        private readonly ProgressDbContext _db;
        private readonly IJwtTokenService _jwt;

        public AuthLoginService(ProgressDbContext db, IJwtTokenService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        public async Task<(bool ok, string message, object? data)> LoginAsync(string employeeNumber, string passwordMd5)
        {
            var user = await _db.Users!.AsNoTracking()
                .FirstOrDefaultAsync(u => u.EmployeeNumber == employeeNumber && u.IsDeleted == 0);
            if (user == null || user.Password != passwordMd5)
                return (false, "login failure，The account password is incorrect!", null);
            if (user.Enable == 0)
                return (false, "The current user is disabled. Contact the administrator", null);

            var roleNames = await (
                from ur in _db.UserRoles
                join r in _db.Roles on ur.RoleId equals r.Id
                where ur.UserId == user.Id
                select r.RoleName!).ToListAsync();

            var token = _jwt.CreateToken(user, roleNames, user.SupplierId);
            return (true, "Login success", new { token });
        }

        public async Task<(string username, List<string> roles, int isSupplierAccount, int? supplierId)> GetInfoAsync(string employeeNumber)
        {
            var user = await _db.Users!.AsNoTracking()
                .FirstOrDefaultAsync(u => u.EmployeeNumber == employeeNumber && u.IsDeleted == 0);
            if (user == null) return ("", new List<string>(), 0, null);
            var roles = await (
                from ur in _db.UserRoles!
                join r in _db.Roles! on ur.RoleId equals r.Id
                where ur.UserId == user.Id
                select r.RoleName!).ToListAsync();
            var list = roles.Where(x => x != null).Cast<string>().ToList();
            return (user.UserName, list, user.IsSupplierAccount, user.SupplierId);
        }

        public async Task<List<DynamicRoutes>> GetDynamicRoutesAsync()
        {
            var menus = await _db.Menus!.AsNoTracking()
                .Where(m => m.Enable == 1)
                .OrderBy(m => m.MenuSort)
                .ToListAsync();
            var menuRoles = await _db.MenuRoles!.AsNoTracking().Include(x => x.Role).ToListAsync();
            var roleByMenu = menuRoles.GroupBy(x => x.MenuId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Role!.RoleName!).Distinct().ToArray());

            List<DynamicRoutes> Build(int parentId)
            {
                var list = new List<DynamicRoutes>();
                foreach (var m in menus.Where(x => x.ParentId == parentId).OrderBy(x => x.MenuSort))
                {
                    roleByMenu.TryGetValue(m.Id, out var roles);
                    roles ??= Array.Empty<string>();
                    var dto = new DynamicRoutes
                    {
                        path = m.Path ?? "",
                        component = m.Url ?? "",
                        redirect = m.Redirect ?? "",
                        name = m.Name ?? "",
                        meta = new Meta
                        {
                            title = m.Title ?? "",
                            elIcon = m.ElIcon ?? "",
                            roles = roles,
                            alwaysShow = m.AlwaysShow,
                            keepAlive = m.KeepAlive
                        },
                        children = Build(m.Id)
                    };
                    if (dto.children?.Count == 0) dto.children = null;
                    list.Add(dto);
                }
                return list;
            }

            return Build(0);
        }
    }
}
