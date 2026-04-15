using Progress.Model.Dto.SystemManagement;

namespace Progress.IService.SystemManagement
{
    public interface ICraftAdminService
    {
        Task<List<CraftRowDto>> ListAsync();

        Task<(int total, List<CraftRowDto> dataList)> QueryAsync(CraftQueryRequest req, CancellationToken ct = default);

        Task<(bool ok, string message)> CreateAsync(CraftCreateDto dto, CancellationToken ct = default);

        Task<(bool ok, string message)> UpdateAsync(int id, CraftUpdateDto dto, CancellationToken ct = default);

        Task<(bool ok, string message)> DeleteAsync(int id, CancellationToken ct = default);
    }
}
