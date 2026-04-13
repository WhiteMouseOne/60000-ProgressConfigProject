using Progress.Model.Dto.Order;

namespace Progress.IService.Business
{
    public interface IHomeStatsService
    {
        Task<HomeDashboardDto> GetDashboardAsync(CancellationToken ct = default);
    }
}
