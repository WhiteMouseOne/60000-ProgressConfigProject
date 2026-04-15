using Microsoft.EntityFrameworkCore;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.SystemManagement;
using Progress.Model.Entitys;
using Progress.Repository;

namespace Progress.Service.SystemManagement
{
    public class CraftAdminService : ICraftAdminService
    {
        private readonly ProgressDbContext _db;

        public CraftAdminService(ProgressDbContext db)
        {
            _db = db;
        }

        public async Task<List<CraftRowDto>> ListAsync()
        {
            return await _db.Crafts!.AsNoTracking().OrderBy(x => x.Code)
                .Select(x => new CraftRowDto { Id = x.Id, Code = x.Code, Name = x.Name })
                .ToListAsync();
        }

        public async Task<(int total, List<CraftRowDto> dataList)> QueryAsync(CraftQueryRequest req, CancellationToken ct = default)
        {
            var q = _db.Crafts!.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(req.name))
                q = q.Where(x => x.Name.Contains(req.name));

            var total = await q.CountAsync(ct);
            var page = Math.Max(1, req.page);
            var size = Math.Max(1, req.pageSize);
            var dataList = await q.OrderBy(x => x.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(x => new CraftRowDto { Id = x.Id, Code = x.Code, Name = x.Name })
                .ToListAsync(ct);
            return (total, dataList);
        }

        public async Task<(bool ok, string message)> CreateAsync(CraftCreateDto dto, CancellationToken ct = default)
        {
            var name = dto.Name.Trim();
            if (string.IsNullOrEmpty(name))
                return (false, "工艺名称不能为空");
            if (await _db.Crafts!.AnyAsync(x => x.Code == dto.Code, ct))
                return (false, "工艺编码已存在");

            _db.Crafts!.Add(new Craft
            {
                Code = dto.Code,
                Name = name,
                RecipeBody = dto.RecipeBody
            });
            await _db.SaveChangesAsync(ct);
            return (true, "创建成功");
        }

        public async Task<(bool ok, string message)> UpdateAsync(int id, CraftUpdateDto dto, CancellationToken ct = default)
        {
            var name = dto.Name.Trim();
            if (string.IsNullOrEmpty(name))
                return (false, "工艺名称不能为空");

            var entity = await _db.Crafts!.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity == null)
                return (false, "工艺不存在");
            if (await _db.Crafts!.AnyAsync(x => x.Code == dto.Code && x.Id != id, ct))
                return (false, "工艺编码已存在");

            entity.Code = dto.Code;
            entity.Name = name;
            entity.RecipeBody = dto.RecipeBody;
            await _db.SaveChangesAsync(ct);
            return (true, "更新成功");
        }

        public async Task<(bool ok, string message)> DeleteAsync(int id, CancellationToken ct = default)
        {
            if (await _db.CraftRecipeSteps!.AnyAsync(s => s.CraftId == id, ct))
                return (false, "该工艺已被配方步序引用，无法删除");

            var entity = await _db.Crafts!.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity == null)
                return (false, "工艺不存在");

            _db.Crafts!.Remove(entity);
            await _db.SaveChangesAsync(ct);
            return (true, "删除成功");
        }
    }
}
