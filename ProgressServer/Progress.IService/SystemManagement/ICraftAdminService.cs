using Progress.Model.Dto.SystemManagement;

namespace Progress.IService.SystemManagement
{
    public interface ICraftAdminService
    {
        Task<List<CraftRowDto>> ListAsync();
    }
}
