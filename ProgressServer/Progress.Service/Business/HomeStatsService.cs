using Microsoft.EntityFrameworkCore;
using Progress.IService.Business;
using Progress.Model;
using Progress.Model.Dto.Order;
using Progress.Model.Entitys;
using Progress.Repository;

namespace Progress.Service.Business
{
    public class HomeStatsService : IHomeStatsService
    {
        private readonly ProgressDbContext _db;
        private readonly IDataScopeService _scope;

        public HomeStatsService(ProgressDbContext db, IDataScopeService scope)
        {
            _db = db;
            _scope = scope;
        }

        public async Task<HomeDashboardDto> GetDashboardAsync(CancellationToken ct = default)
        {
            var setting = await _db.AlertSettings!.AsNoTracking().FirstOrDefaultAsync(ct)
                          ?? new AlertSetting { LeadDays = 3, Enabled = true };
            var today = DateTime.UtcNow.Date;

            var lines = _scope.FilteredOrderLines()
                .Where(x => x.ShippingStatus != OrderShippingStatus.Shipped && x.RequiredDeliveryDate != null);

            var inAlert = await lines
                .Where(x => setting.Enabled
                            && today >= x.RequiredDeliveryDate!.Value.Date.AddDays(-setting.LeadDays))
                .OrderBy(x => x.RequiredDeliveryDate)
                .Take(100)
                .Select(x => new AlertLineDto
                {
                    Id = x.Id,
                    LineNo = x.LineNo,
                    PoNumber = x.PoNumber,
                    PartName = x.PartName,
                    SupplierId = x.SupplierId,
                    SupplierName = x.Supplier!.Name,
                    RequiredDeliveryDate = x.RequiredDeliveryDate,
                    LeadDays = setting.LeadDays
                })
                .ToListAsync(ct);

            var statsBase = _scope.FilteredOrderLines();
            var supplierIds = await statsBase.Select(x => x.SupplierId).Distinct().ToListAsync(ct);
            var names = await _db.Suppliers!.AsNoTracking()
                .Where(s => supplierIds.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, s => s.Name, ct);

            var stats = new List<SupplierStatsDto>();
            foreach (var sid in supplierIds)
            {
                var all = await statsBase.Where(x => x.SupplierId == sid).ToListAsync(ct);
                if (all.Count == 0) continue;
                var shipped = all.Count(x => x.ShippingStatus == OrderShippingStatus.Shipped);
                var overdue = all.Count(x => x.ShippingStatus != OrderShippingStatus.Shipped && x.RequiredDeliveryDate.HasValue &&
                                             x.RequiredDeliveryDate.Value.Date < today);
                stats.Add(new SupplierStatsDto
                {
                    SupplierId = sid,
                    SupplierName = names.GetValueOrDefault(sid),
                    TotalLines = all.Count,
                    ShippedLines = shipped,
                    OverdueNotShipped = overdue,
                    CompletionRate = all.Count > 0 ? Math.Round(100.0 * shipped / all.Count, 2) : 0
                });
            }

            return new HomeDashboardDto { Alerts = inAlert, SupplierStats = stats.OrderBy(s => s.SupplierName).ToList() };
        }
    }
}
