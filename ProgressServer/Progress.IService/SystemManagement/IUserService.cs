using Progress.Model.Dto.SystemManagement;

namespace Progress.IService.SystemManagement
{
    public interface IUserService
    {
        Task<(int total, List<UserListRow> dataList)> GetUserDataAsync(UserGetRequest req, CancellationToken ct = default);

        Task<(bool ok, string message, PersonInfoDto? data)> GetPersonInfoAsync(string employeeNumber, CancellationToken ct = default);

        Task<(bool ok, string message)> AddUserAsync(UserAddDto dto, CancellationToken ct = default);

        Task<(bool ok, string message)> UpdateUserAsync(UserUpdateDto dto, CancellationToken ct = default);

        Task<(bool ok, string message)> DeleteUserAsync(int id, CancellationToken ct = default);

        Task<(bool ok, string message)> BatchDeleteUsersAsync(IReadOnlyList<int> ids, CancellationToken ct = default);

        Task<(bool ok, string message)> ResetPwdOrStatusAsync(ResetPwdOrStatusDto dto, CancellationToken ct = default);

        Task<(bool ok, string message)> UpdateHeadPortraitPathAsync(string employeeNumber, string? relativePath, CancellationToken ct = default);

        Task<(bool ok, string message)> DeleteHeadPortraitAsync(string employeeNumber, CancellationToken ct = default);

        Task<(bool ok, string message)> UpdatePersonalInfoAsync(UpdatePersonalInfoDto dto, CancellationToken ct = default);
    }
}
