using Microsoft.EntityFrameworkCore;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.SystemManagement;
using Progress.Repository;

namespace Progress.Service.SystemManagement
{
    public class CraftAdminService : ICraftAdminService
    {
        private readonly ProgressDbContext _db;

        public CraftAdminService(ProgressDbContext db)
        {
            _db = db;
        }

        public async Task<List<CraftRowDto>> ListAsync()
        {
            return await _db.Crafts!.AsNoTracking().OrderBy(x => x.Code)
                .Select(x => new CraftRowDto { Id = x.Id, Code = x.Code, Name = x.Name })
                .ToListAsync();
        }
    }
}
