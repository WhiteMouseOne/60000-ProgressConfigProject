using Progress.Model.Dto.Order;

namespace Progress.IService.Business
{
    public interface IRepairService
    {
        Task<(bool ok, string message)> CreateAsync(RepairCreateRequest req, CancellationToken ct = default);
    }
}
