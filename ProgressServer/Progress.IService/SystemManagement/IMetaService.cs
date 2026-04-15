using Progress.Model.Dto.SystemManagement;

namespace Progress.IService.SystemManagement
{
    public interface IMetaService
    {
        Task<List<SupplierLiteDto>> GetSuppliersForCurrentUserAsync();

        Task<List<CraftRecipeLiteDto>> GetCraftRecipesAsync(CancellationToken ct = default);

        Task<List<CraftInRecipeStepDto>> GetCraftRecipeCraftsAsync(int craftRecipeId, CancellationToken ct = default);
    }
}
