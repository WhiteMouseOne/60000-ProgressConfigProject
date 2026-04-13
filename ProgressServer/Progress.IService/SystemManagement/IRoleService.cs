using Progress.Model.Dto.SystemManagement;

namespace Progress.IService.SystemManagement
{
    public interface IRoleService
    {
        Task<(int total, List<RoleListRow> dataList)> GetRoleDataAsync(RoleGetRequest req, CancellationToken ct = default);

        Task<List<RoleDropdownDto>> GetAliveRolesForSelectAsync(CancellationToken ct = default);

        Task<(List<RoleDropdownDto> roles, int roleIds)> GetRolesAndUserRoleIdAsync(int userId, CancellationToken ct = default);

        Task<(bool ok, string message)> AddRoleAsync(RoleAddDto dto, CancellationToken ct = default);

        Task<(bool ok, string message)> UpdateRoleAsync(RoleUpdateDto dto, CancellationToken ct = default);

        Task<(bool ok, string message)> DeleteRoleAsync(int id, CancellationToken ct = default);

        Task<(bool ok, string message)> BatchDeleteRolesAsync(int[]? ids, CancellationToken ct = default);

        Task<(bool ok, string message)> ChangeRoleEnableAsync(RoleChangeEnableDto dto, CancellationToken ct = default);

        Task<(bool ok, string message)> UpdateRoleMenuScopeAsync(RoleMenuScopeDto dto, CancellationToken ct = default);
    }
}
