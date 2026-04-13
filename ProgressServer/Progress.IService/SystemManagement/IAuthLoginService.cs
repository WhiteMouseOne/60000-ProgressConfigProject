using Progress.Model.Dto.Menu;

namespace Progress.IService.SystemManagement
{
    public interface IAuthLoginService
    {
        Task<(bool ok, string message, object? data)> LoginAsync(string employeeNumber, string passwordMd5);
        Task<(string username, List<string> roles)> GetInfoAsync(string employeeNumber);
        Task<List<DynamicRoutes>> GetDynamicRoutesAsync();
    }
}
