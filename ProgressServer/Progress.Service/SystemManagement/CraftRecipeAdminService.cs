using Microsoft.EntityFrameworkCore;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.SystemManagement;
using Progress.Model.Entitys;
using Progress.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Service.SystemManagement
{
    public class CraftRecipeAdminService : ICraftRecipeAdminService
    {
        private readonly ProgressDbContext _db;
        public CraftRecipeAdminService(ProgressDbContext db)
        {
            _db = db;
        }
        public async Task<List<CraftRecipeRowDto>> ListAsync(CancellationToken ct = default)
        {
            return await _db.CraftRecipes!.AsNoTracking()
               .OrderBy(x => x.Id)
               .Select(x => new CraftRecipeRowDto
               {
                   Id = x.Id,
                   Code = x.Code,
                   Name = x.Name,
               }).ToListAsync(ct);
        }

        public async Task<(int total, List<CraftRecipeRowDto> list)> QueryAsync(CraftRecipeQueryRequest req, CancellationToken ct = default)
        {
            var q = _db.CraftRecipes!.AsNoTracking();
            //判断配方名是否存在
            if (!string.IsNullOrWhiteSpace(req.Name))
                q= q.Where(x=>x.Name.Contains(req.Name));
            var total = await q.CountAsync(ct);
            var page = Math.Max(1,req.page);
            var size = Math.Max(1, req.pageSize);

            var dataList = await q.OrderBy(x => x.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(x => new CraftRecipeRowDto
                {
                    Id= x.Id,
                    Code = x.Code,
                    Name =x.Name
                }).ToListAsync(ct);
            return (total, dataList);
        }

        public async Task<(bool ok, string message)> CreateAsync(CraftRecipeCreateDto dto, CancellationToken ct = default)
        {
            //判断是否满足创建条件
            var name = dto.Name.Trim();
            if (string.IsNullOrEmpty(name))
                return (false, "配方名称不能为空");
            if (await _db.CraftRecipes!.AnyAsync(x => x.Code == dto.Code,ct))
                return (false, "配方编码已存在");

            //添加进入数据库
            _db.CraftRecipes!.Add(new CraftRecipe { Code = dto.Code, Name = name });
            await _db.SaveChangesAsync(ct);
            return (true, "创建成功");
        }

        public async Task<(bool ok, string messagq)> UpdateAsync(int id, CraftRecipeUpdateDto dto, CancellationToken ct = default)
        {
            var name = dto.Name.Trim();
            if (string.IsNullOrEmpty(name))
                return (false, "配方名不能为空");
            var entity = await _db.CraftRecipes!.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity == null)
                return (false, "配方不存在");
            //if (await _db.CraftRecipes!.AnyAsync(x => x.Code == dto.Code))
            //    return (false, "配方码不能重复");
            //更新内容
            entity.Code = dto.Code;
            entity.Name = name;
            await _db.SaveChangesAsync(ct);
            return (true, "更新成功");
        }
        public async Task<(bool ok, string message)> DeleteAsync(int id, CancellationToken ct = default)
        {
            //被订单行引用时不允许删除
            if (await _db.WorkpieceOrderLines!.AnyAsync(x => x.CraftRecipeId == id, ct))
                return (false, "该配方已被订单引用，无法删除");
            //在工艺步序中存在时，无法删除
            if (await _db.CraftRecipeSteps!.AnyAsync(x => x.CraftRecipeId == id, ct))
                return (false, "该配方已存在工艺步序中，请先在工艺步序中删除");

            //开始删除
            var entity = await _db.CraftRecipes!.FirstOrDefaultAsync(x => x.Id == id,ct);
            if (entity == null) return (false, "配方不存在");
            _db.CraftRecipes!.Remove(entity);
            await _db.SaveChangesAsync(ct);
            return (true, "删除成功");
        }
    }
}
