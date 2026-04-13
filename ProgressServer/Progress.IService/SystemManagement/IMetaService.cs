using Progress.Model.Dto.SystemManagement;

namespace Progress.IService.SystemManagement
{
    public interface IMetaService
    {
        Task<List<SupplierLiteDto>> GetSuppliersForCurrentUserAsync();
    }
}
