using Microsoft.EntityFrameworkCore;
using Progress.IService;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.SystemManagement;
using Progress.Repository;

namespace Progress.Service.SystemManagement
{
    public class MetaService : IMetaService
    {
        private readonly ProgressDbContext _db;
        private readonly ICurrentUser _user;

        public MetaService(ProgressDbContext db, ICurrentUser user)
        {
            _db = db;
            _user = user;
        }

        public async Task<List<SupplierLiteDto>> GetSuppliersForCurrentUserAsync()
        {
            if (_user.IsAdmin || _user.Roles.Contains("Supervisor"))
            {
                return await _db.Suppliers!.AsNoTracking().OrderBy(x => x.SupplierNumber)
                    .Select(x => new SupplierLiteDto { Id = x.Id, SupplierNumber = x.SupplierNumber, Name = x.Name })
                    .ToListAsync();
            }
            if (_user.SupplierId is { } sid)
            {
                return await _db.Suppliers!.AsNoTracking().Where(x => x.Id == sid)
                    .Select(x => new SupplierLiteDto { Id = x.Id, SupplierNumber = x.SupplierNumber, Name = x.Name })
                    .ToListAsync();
            }
            return new List<SupplierLiteDto>();
        }

        public async Task<List<CraftRecipeLiteDto>> GetCraftRecipesAsync(CancellationToken ct = default)
        {
            return await _db.CraftRecipes!.AsNoTracking().OrderBy(x => x.Code)
                .Select(x => new CraftRecipeLiteDto { Id = x.Id, Code = x.Code, Name = x.Name })
                .ToListAsync(ct);
        }

        public async Task<List<CraftInRecipeStepDto>> GetCraftRecipeCraftsAsync(int craftRecipeId, CancellationToken ct = default)
        {
            if (!await _db.CraftRecipes!.AsNoTracking().AnyAsync(r => r.Id == craftRecipeId, ct))
                return new List<CraftInRecipeStepDto>();

            return await (
                from s in _db.CraftRecipeSteps!.AsNoTracking()
                join c in _db.Crafts!.AsNoTracking() on s.CraftId equals c.Id
                where s.CraftRecipeId == craftRecipeId
                orderby s.StepOrder
                select new CraftInRecipeStepDto
                {
                    CraftId = c.Id,
                    CraftCode = c.Code,
                    CraftName = c.Name,
                    StepOrder = s.StepOrder
                }).ToListAsync(ct);
        }
    }
}
