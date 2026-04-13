using Progress.Model.Dto.Order;

namespace Progress.IService.SystemManagement
{
    public interface IAlertSettingService
    {
        Task<AlertSettingDto?> GetAsync();
        Task SaveAsync(AlertSettingDto dto);
    }
}
