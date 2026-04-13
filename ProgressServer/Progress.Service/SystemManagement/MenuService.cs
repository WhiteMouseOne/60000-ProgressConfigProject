using Microsoft.EntityFrameworkCore;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.Menu;
using Progress.Model.Entitys;
using Progress.Repository;

namespace Progress.Service.SystemManagement
{
    public class MenuService : IMenuService
    {
        private const int AdminRoleId = 1;

        private readonly ProgressDbContext _db;

        public MenuService(ProgressDbContext db)
        {
            _db = db;
        }

        public async Task<MenuTreeSelectResult> GetMenuTreeSelectAsync(int roleId, CancellationToken ct = default)
        {
            var flat = await _db.Menus!.AsNoTracking()
                .Where(m => m.Enable == 1)
                .OrderBy(m => m.Id)
                .ToListAsync(ct);

            var menus = BuildMenuTreeForSelect(flat, 0);
            var checkKeys = await _db.MenuRoles!.AsNoTracking()
                .Where(mr => mr.RoleId == roleId)
                .Select(mr => mr.MenuId)
                .Distinct()
                .ToListAsync(ct);

            return new MenuTreeSelectResult { menus = menus, checkKeys = checkKeys };
        }

        public async Task<List<MenuTreeRowDto>> GetMenuDataAsync(MenuQueryRequest req, CancellationToken ct = default)
        {
            var all = await _db.Menus!.AsNoTracking().OrderBy(m => m.Id).ToListAsync(ct);
            var objMenu = all.ToList();

            if (!string.IsNullOrWhiteSpace(req.enable))
            {
                var en = req.enable!.Trim();
                if (en is "1" or "true" or "True")
                    objMenu = objMenu.Where(i => i.Enable == 1).ToList();
                else if (en is "0" or "false" or "False")
                    objMenu = objMenu.Where(i => i.Enable == 0).ToList();
            }

            if (!string.IsNullOrWhiteSpace(req.menuName))
            {
                objMenu = objMenu.Where(i => (i.Title ?? "").Contains(req.menuName!, StringComparison.Ordinal)).ToList();
                objMenu = RepParentMenu(all, objMenu);
            }

            return BuildMenuRowTree(objMenu, 0);
        }

        public async Task<AllMenuTreeResult> GetAllMenuTreeSelectAsync(CancellationToken ct = default)
        {
            var all = await _db.Menus!.AsNoTracking()
                .Where(m => m.Enable == 1)
                .OrderBy(m => m.MenuSort ?? "")
                .ThenBy(m => m.Id)
                .ToListAsync(ct);
            var menus = BuildMenuRowTree(all, 0);
            return new AllMenuTreeResult { menus = menus };
        }

        public async Task<(bool ok, string message)> AddMenuAsync(MenuAddDto dto, CancellationToken ct = default)
        {
            var enable = ParseIntString(dto.enable, 1);
            var now = DateTime.UtcNow;
            var menu = new Menu
            {
                ParentId = dto.parentId,
                Name = dto.name,
                Title = dto.title,
                Path = dto.path,
                ElIcon = dto.elIcon,
                Url = dto.url,
                MenuType = dto.menuType,
                MenuSort = dto.menuSort,
                KeepAlive = dto.keepAlive,
                Enable = enable,
                CreateBy = dto.createBy,
                CreateTime = now,
                Redirect = dto.Redirect ?? "",
                AlwaysShow = dto.alwaysShow
            };
            _db.Menus!.Add(menu);
            await _db.SaveChangesAsync(ct);

            await EnsureAdminHasMenuAsync(menu.Id, ct);
            return (true, "Add Success!");
        }

        public async Task<(bool ok, string message)> UpdateMenuAsync(MenuUpdateDto dto, CancellationToken ct = default)
        {
            var menu = await _db.Menus!.FirstOrDefaultAsync(m => m.Id == dto.id, ct);
            if (menu == null)
                return (false, "菜单不存在");

            menu.ParentId = dto.parentId;
            menu.Name = dto.name;
            menu.Title = dto.title;
            menu.Path = dto.path;
            menu.ElIcon = dto.elIcon;
            menu.Url = dto.url;
            menu.MenuType = dto.menuType;
            menu.MenuSort = dto.menuSort;
            menu.KeepAlive = ParseKeepAlive(dto.keepAlive);
            menu.Enable = ParseIntString(dto.enable, menu.Enable);
            menu.UpdateBy = dto.updateBy;
            menu.UpdateTime = DateTime.UtcNow;
            menu.Redirect = dto.Redirect ?? menu.Redirect;
            menu.AlwaysShow = dto.alwaysShow;

            await _db.SaveChangesAsync(ct);
            return (true, "Update Success!");
        }

        public async Task<(bool ok, string message)> DeleteMenuAsync(int id, CancellationToken ct = default)
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                await DeleteMenuRecursiveAsync(id, ct);
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }

            return (true, "Delete Success!");
        }

        public async Task<(bool ok, string message)> BatchDeleteMenusAsync(IReadOnlyList<int> ids, CancellationToken ct = default)
        {
            if (ids == null || ids.Count == 0)
                return (false, "请选择要删除的菜单");

            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                foreach (var id in ids.Distinct())
                    await DeleteMenuRecursiveAsync(id, ct);
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }

            return (true, "Delete Success!");
        }

        public async Task<(bool ok, string message)> ChangeMenuStatusAsync(UpdateMenuStatusDto dto, CancellationToken ct = default)
        {
            var menu = await _db.Menus!.FirstOrDefaultAsync(m => m.Id == dto.id, ct);
            if (menu == null)
                return (false, "菜单不存在");

            menu.KeepAlive = dto.keepAlive;
            menu.UpdateTime = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return (true, "Operate Success!");
        }

        private async Task DeleteMenuRecursiveAsync(int id, CancellationToken ct)
        {
            var menu = await _db.Menus!.FirstOrDefaultAsync(m => m.Id == id, ct);
            if (menu == null) return;

            var children = await _db.Menus!.Where(m => m.ParentId == id).Select(m => m.Id).ToListAsync(ct);
            foreach (var cid in children)
                await DeleteMenuRecursiveAsync(cid, ct);

            await _db.MenuRoles!.Where(mr => mr.MenuId == id).ExecuteDeleteAsync(ct);
            var again = await _db.Menus!.FirstOrDefaultAsync(m => m.Id == id, ct);
            if (again != null)
                _db.Menus!.Remove(again);
        }

        private async Task EnsureAdminHasMenuAsync(int menuId, CancellationToken ct)
        {
            var exists = await _db.MenuRoles!.AnyAsync(mr => mr.RoleId == AdminRoleId && mr.MenuId == menuId, ct);
            if (exists) return;
            _db.MenuRoles!.Add(new MenuRole { MenuId = menuId, RoleId = AdminRoleId });
            await _db.SaveChangesAsync(ct);
        }

        private static List<Menu> RepParentMenu(List<Menu> all, List<Menu> filtered)
        {
            var byId = all.ToDictionary(m => m.Id);
            var result = new Dictionary<int, Menu>();
            foreach (var m in filtered)
                result[m.Id] = m;
            var queue = new Queue<Menu>(filtered);
            while (queue.Count > 0)
            {
                var m = queue.Dequeue();
                if (m.ParentId > 0 && byId.TryGetValue(m.ParentId, out var p) && !result.ContainsKey(p.Id))
                {
                    result[p.Id] = p;
                    queue.Enqueue(p);
                }
            }
            return result.Values.ToList();
        }

        private static List<MenuTreeRowDto> BuildMenuRowTree(List<Menu> flat, int parentId)
        {
            var children = flat
                .Where(m => m.ParentId == parentId)
                .OrderBy(m => m.MenuSort ?? "")
                .ThenBy(m => m.Id)
                .ToList();
            var list = new List<MenuTreeRowDto>();
            foreach (var m in children)
            {
                var node = MapToRow(m);
                var sub = BuildMenuRowTree(flat, m.Id);
                if (sub.Count > 0)
                    node.children = sub;
                list.Add(node);
            }
            return list;
        }

        private static MenuTreeRowDto MapToRow(Menu m) => new()
        {
            id = m.Id,
            parentId = m.ParentId,
            name = m.Name,
            title = m.Title,
            path = m.Path,
            elIcon = m.ElIcon,
            url = m.Url,
            menuType = m.MenuType,
            menuSort = m.MenuSort,
            keepAlive = m.KeepAlive,
            enable = m.Enable,
            createBy = m.CreateBy,
            createTime = m.CreateTime,
            updateBy = m.UpdateBy,
            updateTime = m.UpdateTime,
            alwaysShow = m.AlwaysShow,
            redirect = m.Redirect
        };

        private static List<MenuTreeNode> BuildMenuTreeForSelect(List<Menu> flat, int parentId)
        {
            var children = flat
                .Where(m => m.ParentId == parentId)
                .OrderBy(m => m.MenuSort ?? "")
                .ThenBy(m => m.Id)
                .ToList();
            var list = new List<MenuTreeNode>();
            foreach (var m in children)
            {
                var node = new MenuTreeNode
                {
                    id = m.Id,
                    title = m.Title ?? m.Name ?? ""
                };
                var sub = BuildMenuTreeForSelect(flat, m.Id);
                if (sub.Count > 0)
                    node.children = sub;
                list.Add(node);
            }
            return list;
        }

        private static int ParseIntString(string? s, int fallback)
        {
            if (string.IsNullOrWhiteSpace(s)) return fallback;
            return int.TryParse(s.Trim(), out var v) ? v : fallback;
        }

        private static int ParseKeepAlive(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            if (s == "1" || s.Equals("true", StringComparison.OrdinalIgnoreCase)) return 1;
            if (int.TryParse(s, out var n)) return n != 0 ? 1 : 0;
            return 0;
        }
    }
}
