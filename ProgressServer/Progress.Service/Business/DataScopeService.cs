using Microsoft.EntityFrameworkCore;
using Progress.IService;
using Progress.IService.Business;
using Progress.Model.Entitys;
using Progress.Repository;

namespace Progress.Service.Business
{
    public class DataScopeService : IDataScopeService
    {
        private readonly ProgressDbContext _db;
        private readonly ICurrentUser _user;

        public DataScopeService(ProgressDbContext db, ICurrentUser user)
        {
            _db = db;
            _user = user;
        }

        public IQueryable<WorkpieceOrderLine> FilteredOrderLines()
        {
            var q = _db.WorkpieceOrderLines!.AsNoTracking().Include(x => x.Supplier);
            if (_user.IsAdmin) return q;

            if (_user.Roles.Contains("Supervisor")) return q;

            if (_user.IsSupplierAccount == 1 && _user.SupplierId is { } sid)
                return q.Where(x => x.SupplierId == sid);

            return q.Where(x => false);
        }

        public async Task<bool> CanAccessLineAsync(int lineId, CancellationToken ct = default)
        {
            return await FilteredOrderLines().AnyAsync(x => x.Id == lineId, ct);
        }
    }
}
