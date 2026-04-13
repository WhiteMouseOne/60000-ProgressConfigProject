using Microsoft.EntityFrameworkCore;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.SystemManagement;
using Progress.Model.Entitys;
using Progress.Repository;

namespace Progress.Service.SystemManagement
{
    public class RoleService : IRoleService
    {
        private readonly ProgressDbContext _db;

        public RoleService(ProgressDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 查询角色列表，支持对角色名称、角色排序和启用状态进行过滤。
        /// </summary>
        /// <param name="req"></param>
        /// <param name="ct"></param>
        /// <returns>返回吗做条件的角色总数和分页后的角色列表</returns>
        public async Task<(int total, List<RoleListRow> dataList)> GetRoleDataAsync(RoleGetRequest req, CancellationToken ct = default)
        {
            var q = _db.Roles!.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(req.roleName))
                q = q.Where(r => r.RoleName.Contains(req.roleName));

            var total = await q.CountAsync(ct);
            var page = Math.Max(1, req.page);
            var size = Math.Max(1, req.size);
            var list = await q
                .OrderBy(r => r.RoleSort)
                .ThenBy(r => r.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(r => new RoleListRow
                {
                    id = r.Id,
                    roleName = r.RoleName,
                    description = r.Description,
                    roleSort = r.RoleSort,
                    enable = r.Enable,
                    createBy = r.CreateBy,
                    createTime = r.CreateTime,
                    updateBy = r.UpdateBy,
                    updateTime = r.UpdateTime
                })
                .ToListAsync(ct);

            return (total, list);
        }

        public async Task<List<RoleDropdownDto>> GetAliveRolesForSelectAsync(CancellationToken ct = default)
        {
            return await _db.Roles!.AsNoTracking()
                .Where(r => r.Enable == 1)
                .OrderBy(r => r.RoleSort)
                .Select(r => new RoleDropdownDto
                {
                    id = r.Id,
                    roleName = r.RoleName,
                    description = r.Description,
                    enable = r.Enable
                })
                .ToListAsync(ct);
        }

        public async Task<(List<RoleDropdownDto> roles, int roleIds)> GetRolesAndUserRoleIdAsync(int userId, CancellationToken ct = default)
        {
            var roles = await GetAliveRolesForSelectAsync(ct);

            var ur = await _db.UserRoles!.AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => (int?)x.RoleId)
                .FirstOrDefaultAsync(ct);

            return (roles, ur ?? -1);
        }

        public async Task<(bool ok, string message)> AddRoleAsync(RoleAddDto body, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(body.roleName))
                return (false, "角色名称不能为空");

            var exists = await _db.Roles!.AnyAsync(r => r.RoleName == body.roleName.Trim(), ct);
            if (exists)
                return (false, "角色名称已存在");

            var now = DateTime.UtcNow;
            var role = new Role
            {
                RoleName = body.roleName.Trim(),
                RoleSort = body.roleSort,
                Enable = body.enable,
                CreateBy = body.createBy,
                CreateTime = now
            };
            _db.Roles!.Add(role);
            await _db.SaveChangesAsync(ct);
            return (true, "新增成功");
        }

        public async Task<(bool ok, string message)> UpdateRoleAsync(RoleUpdateDto body, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(body.roleName))
                return (false, "角色名称不能为空");

            var role = await _db.Roles!.FirstOrDefaultAsync(r => r.Id == body.id, ct);
            if (role == null)
                return (false, "角色不存在");

            var name = body.roleName.Trim();
            var dup = await _db.Roles!.AnyAsync(r => r.RoleName == name && r.Id != body.id, ct);
            if (dup)
                return (false, "角色名称已存在");

            role.RoleName = name;
            role.RoleSort = body.roleSort;
            role.Enable = body.enable;
            role.UpdateBy = body.updateBy;
            role.UpdateTime = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return (true, "修改成功");
        }

        public async Task<(bool ok, string message)> DeleteRoleAsync(int id, CancellationToken ct = default)
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var role = await _db.Roles!.FirstOrDefaultAsync(r => r.Id == id, ct);
            if (role == null)
            {
                await tx.RollbackAsync(ct);
                return (false, "角色不存在");
            }

            await _db.UserRoles!.Where(ur => ur.RoleId == id).ExecuteDeleteAsync(ct);
            await _db.MenuRoles!.Where(mr => mr.RoleId == id).ExecuteDeleteAsync(ct);
            _db.Roles!.Remove(role);
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return (true, "删除成功");
        }

        public async Task<(bool ok, string message)> BatchDeleteRolesAsync(int[]? ids, CancellationToken ct = default)
        {
            if (ids == null || ids.Length == 0)
                return (false, "请选择要删除的角色");

            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            foreach (var id in ids.Distinct())
            {
                var role = await _db.Roles!.FirstOrDefaultAsync(r => r.Id == id, ct);
                if (role == null) continue;
                await _db.UserRoles!.Where(ur => ur.RoleId == id).ExecuteDeleteAsync(ct);
                await _db.MenuRoles!.Where(mr => mr.RoleId == id).ExecuteDeleteAsync(ct);
                _db.Roles!.Remove(role);
            }

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return (true, "批量删除成功");
        }

        public async Task<(bool ok, string message)> ChangeRoleEnableAsync(RoleChangeEnableDto body, CancellationToken ct = default)
        {
            var role = await _db.Roles!.FirstOrDefaultAsync(r => r.Id == body.id, ct);
            if (role == null)
                return (false, "角色不存在");

            role.Enable = body.enable;
            role.UpdateTime = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return (true, "状态已更新");
        }

        public async Task<(bool ok, string message)> UpdateRoleMenuScopeAsync(RoleMenuScopeDto body, CancellationToken ct = default)
        {
            var role = await _db.Roles!.AsNoTracking().FirstOrDefaultAsync(r => r.Id == body.roleId, ct);
            if (role == null)
                return (false, "角色不存在");

            var menuIds = body.menuIds?.Distinct().ToList() ?? new List<int>();
            if (menuIds.Count > 0)
            {
                var valid = await _db.Menus!.Where(m => menuIds.Contains(m.Id)).Select(m => m.Id).ToListAsync(ct);
                var invalid = menuIds.Except(valid).ToList();
                if (invalid.Count > 0)
                    return (false, "包含无效菜单 Id");
            }

            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            await _db.MenuRoles!.Where(mr => mr.RoleId == body.roleId).ExecuteDeleteAsync(ct);

            foreach (var mid in menuIds)
                _db.MenuRoles!.Add(new MenuRole { MenuId = mid, RoleId = body.roleId });

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return (true, "菜单权限已保存");
        }
    }
}
