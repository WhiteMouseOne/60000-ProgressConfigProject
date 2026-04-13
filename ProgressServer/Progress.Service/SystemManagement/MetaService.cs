using Microsoft.EntityFrameworkCore;
using Progress.IService;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.SystemManagement;
using Progress.Repository;

namespace Progress.Service.SystemManagement
{
    public class MetaService : IMetaService
    {
        private readonly ProgressDbContext _db;
        private readonly ICurrentUser _user;

        public MetaService(ProgressDbContext db, ICurrentUser user)
        {
            _db = db;
            _user = user;
        }

        public async Task<List<SupplierLiteDto>> GetSuppliersForCurrentUserAsync()
        {
            if (_user.IsAdmin || _user.Roles.Contains("Supervisor"))
            {
                return await _db.Suppliers!.AsNoTracking().OrderBy(x => x.SupplierNumber)
                    .Select(x => new SupplierLiteDto { Id = x.Id, SupplierNumber = x.SupplierNumber, Name = x.Name })
                    .ToListAsync();
            }
            if (_user.SupplierId is { } sid)
            {
                return await _db.Suppliers!.AsNoTracking().Where(x => x.Id == sid)
                    .Select(x => new SupplierLiteDto { Id = x.Id, SupplierNumber = x.SupplierNumber, Name = x.Name })
                    .ToListAsync();
            }
            return new List<SupplierLiteDto>();
        }
    }
}
