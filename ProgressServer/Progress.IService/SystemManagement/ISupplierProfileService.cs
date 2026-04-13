using Progress.Model.Dto.SystemManagement;

namespace Progress.IService.SystemManagement
{
    public interface ISupplierProfileService
    {
        Task<SupplierProfileDto?> GetAsync(CancellationToken ct = default);
        Task<(bool ok, string message)> UpdateAsync(SupplierProfileUpdateDto dto, CancellationToken ct = default);
    }
}
