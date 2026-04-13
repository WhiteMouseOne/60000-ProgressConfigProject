using Progress.Model.Dto.SystemManagement;

namespace Progress.IService.SystemManagement
{
    public interface ISupplierAdminService
    {
        Task<List<SupplierRowDto>> ListAsync();
        Task<(int total, List<SupplierListRow> list)> QueryAsync(SupplierQueryRequest req, CancellationToken ct = default);
        Task<(bool ok, string message)> CreateAsync(SupplierCreateDto dto, CancellationToken ct = default);
        Task<(bool ok, string message)> UpdateAsync(int id, SupplierAdminUpdateDto dto, CancellationToken ct = default);
        Task<(bool ok, string message)> DeleteAsync(int id, CancellationToken ct = default);
        Task<(bool ok, string message, int deleted)> BatchDeleteAsync(IReadOnlyList<int> ids, CancellationToken ct=default);
    }
}
