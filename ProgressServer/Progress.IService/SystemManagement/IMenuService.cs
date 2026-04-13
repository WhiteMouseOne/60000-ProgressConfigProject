using Progress.Model.Dto.Menu;

namespace Progress.IService.SystemManagement
{
    public interface IMenuService
    {
        Task<MenuTreeSelectResult> GetMenuTreeSelectAsync(int roleId, CancellationToken ct = default);

        Task<List<MenuTreeRowDto>> GetMenuDataAsync(MenuQueryRequest req, CancellationToken ct = default);

        Task<AllMenuTreeResult> GetAllMenuTreeSelectAsync(CancellationToken ct = default);

        Task<(bool ok, string message)> AddMenuAsync(MenuAddDto dto, CancellationToken ct = default);

        Task<(bool ok, string message)> UpdateMenuAsync(MenuUpdateDto dto, CancellationToken ct = default);

        Task<(bool ok, string message)> DeleteMenuAsync(int id, CancellationToken ct = default);

        Task<(bool ok, string message)> BatchDeleteMenusAsync(IReadOnlyList<int> ids, CancellationToken ct = default);

        Task<(bool ok, string message)> ChangeMenuStatusAsync(UpdateMenuStatusDto dto, CancellationToken ct = default);
    }
}
