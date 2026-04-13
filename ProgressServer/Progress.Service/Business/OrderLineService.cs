using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Progress.Common;
using Progress.IService;
using Progress.IService.Business;
using Progress.Model;
using Progress.Model.Dto.Order;
using Progress.Model.Entitys;
using Progress.Repository;

namespace Progress.Service.Business
{
    public class OrderLineService : IOrderLineService
    {
        private readonly ProgressDbContext _db;
        private readonly IDataScopeService _scope;
        private readonly ICurrentUser _user;
        private readonly PoValidationOptions _po;

        public OrderLineService(ProgressDbContext db, IDataScopeService scope, ICurrentUser user, IOptions<PoValidationOptions> poOpt)
        {
            _db = db;
            _scope = scope;
            _user = user;
            _po = poOpt.Value;
        }

        private void ValidatePo(string poNumber)
        {
            if (string.IsNullOrWhiteSpace(_po.Pattern)) return;
            if (!Regex.IsMatch(poNumber, _po.Pattern))
                throw new InvalidOperationException("PO 编号格式不符合配置规则");
        }

        public async Task<PagedResult<OrderLineDto>> QueryAsync(OrderLineQuery query, CancellationToken ct = default)
        {
            var q = _scope.FilteredOrderLines();
            if (!string.IsNullOrWhiteSpace(query.PoNumber))
                q = q.Where(x => x.PoNumber.Contains(query.PoNumber));
            if (!string.IsNullOrWhiteSpace(query.ProjectCode))
                q = q.Where(x => x.ProjectCode.Contains(query.ProjectCode));
            if (!string.IsNullOrWhiteSpace(query.DrawingNumber))
                q = q.Where(x => x.DrawingNumber.Contains(query.DrawingNumber));
            if (!string.IsNullOrWhiteSpace(query.PartName))
                q = q.Where(x => x.PartName.Contains(query.PartName));
            if (query.SupplierId is { } sid && (_user.IsAdmin || _user.Roles.Contains("Supervisor")))
                q = q.Where(x => x.SupplierId == sid);

            var total = await q.CountAsync(ct);
            var page = Math.Max(1, query.Page);
            var size = Math.Clamp(query.PageSize, 1, 200);
            var items = await q.OrderByDescending(x => x.CreateTime)
                .Skip((page - 1) * size).Take(size)
                .Select(x => new OrderLineDto
                {
                    Id = x.Id,
                    LineNo = x.LineNo,
                    PoNumber = x.PoNumber,
                    ProjectCode = x.ProjectCode,
                    DrawingNumber = x.DrawingNumber,
                    PartName = x.PartName,
                    Material = x.Material,
                    SupplierId = x.SupplierId,
                    SupplierName = x.Supplier!.Name,
                    Quantity = x.Quantity,
                    Unit = x.Unit,
                    ReceivedQuantity = x.ReceivedQuantity,
                    RequiredDeliveryDate = x.RequiredDeliveryDate,
                    LatestCraftCode = x.LatestCraftCode,
                    VendorUpdatedAt = x.VendorUpdatedAt,
                    VendorEstimatedDeliveryDate = x.VendorEstimatedDeliveryDate,
                    ShippedQuantity = x.ShippedQuantity,
                    ShippingStatus = x.ShippingStatus,
                    SupplierNotes = x.SupplierNotes,
                    ActualDeliveryDate = x.ActualDeliveryDate,
                    RepairStatus = x.RepairStatus,
                    RepairCreatedAt = x.RepairCreatedAt,
                    RepairStartedAt = x.RepairStartedAt,
                    RepairShippedAt = x.RepairShippedAt,
                    CreateTime = x.CreateTime
                })
                .ToListAsync(ct);
            return new PagedResult<OrderLineDto> { Items = items, Total = total };
        }

        public async Task<OrderLineDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            if (!await _scope.CanAccessLineAsync(id, ct)) return null;
            var x = await _db.WorkpieceOrderLines!.AsNoTracking().Include(s => s.Supplier)
                .FirstOrDefaultAsync(l => l.Id == id, ct);
            if (x == null) return null;
            return new OrderLineDto
            {
                Id = x.Id,
                LineNo = x.LineNo,
                PoNumber = x.PoNumber,
                ProjectCode = x.ProjectCode,
                DrawingNumber = x.DrawingNumber,
                PartName = x.PartName,
                Material = x.Material,
                SupplierId = x.SupplierId,
                SupplierName = x.Supplier?.Name,
                Quantity = x.Quantity,
                Unit = x.Unit,
                ReceivedQuantity = x.ReceivedQuantity,
                RequiredDeliveryDate = x.RequiredDeliveryDate,
                LatestCraftCode = x.LatestCraftCode,
                VendorUpdatedAt = x.VendorUpdatedAt,
                VendorEstimatedDeliveryDate = x.VendorEstimatedDeliveryDate,
                ShippedQuantity = x.ShippedQuantity,
                ShippingStatus = x.ShippingStatus,
                SupplierNotes = x.SupplierNotes,
                ActualDeliveryDate = x.ActualDeliveryDate,
                RepairStatus = x.RepairStatus,
                RepairCreatedAt = x.RepairCreatedAt,
                RepairStartedAt = x.RepairStartedAt,
                RepairShippedAt = x.RepairShippedAt,
                CreateTime = x.CreateTime
            };
        }

        public async Task<int> CreateAsync(OrderLineCreateOrEdit input, CancellationToken ct = default)
        {
            if (!_user.IsAdmin && !_user.Roles.Contains("Supervisor"))
                throw new UnauthorizedAccessException();
            ValidatePo(input.PoNumber);
            if (!await _db.Suppliers!.AnyAsync(s => s.Id == input.SupplierId, ct))
                throw new InvalidOperationException("供应商不存在");
            var maxNo = await _db.WorkpieceOrderLines!.Where(x => x.PoNumber == input.PoNumber)
                .MaxAsync(x => (int?)x.LineNo, ct) ?? 0;
            var line = new WorkpieceOrderLine
            {
                LineNo = maxNo + 1,
                PoNumber = input.PoNumber.Trim(),
                ProjectCode = input.ProjectCode.Trim(),
                DrawingNumber = input.DrawingNumber.Trim(),
                PartName = input.PartName.Trim(),
                Material = string.IsNullOrWhiteSpace(input.Material) ? null : input.Material.Trim(),
                SupplierId = input.SupplierId,
                Quantity = input.Quantity,
                Unit = input.Unit,
                ReceivedQuantity = input.ReceivedQuantity,
                RequiredDeliveryDate = input.RequiredDeliveryDate,
                LatestCraftCode = input.LatestCraftCode,
                ShippedQuantity = input.Quantity,
                ShippingStatus = OrderShippingStatus.NotFilled,
                SupplierNotes = input.SupplierNotes,
                ActualDeliveryDate = input.ActualDeliveryDate,
                RepairStatus = OrderRepairStatus.None,
                CreateTime = DateTime.UtcNow
            };
            _db.WorkpieceOrderLines!.Add(line);
            await _db.SaveChangesAsync(ct);
            return line.Id;
        }

        public async Task<bool> UpdateAsync(OrderLineCreateOrEdit input, CancellationToken ct = default)
        {
            if (input.Id is not { } id) throw new InvalidOperationException("缺少行 Id");
            if (!_user.IsAdmin && !_user.Roles.Contains("Supervisor")) throw new UnauthorizedAccessException();
            if (!await _scope.CanAccessLineAsync(id, ct)) return false;
            var line = await _db.WorkpieceOrderLines!.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (line == null) return false;
            ValidatePo(input.PoNumber);
            line.PoNumber = input.PoNumber.Trim();
            line.ProjectCode = input.ProjectCode.Trim();
            line.DrawingNumber = input.DrawingNumber.Trim();
            line.PartName = input.PartName.Trim();
            line.Material = string.IsNullOrWhiteSpace(input.Material) ? null : input.Material.Trim();
            line.SupplierId = input.SupplierId;
            line.Quantity = input.Quantity;
            line.Unit = input.Unit;
            line.ReceivedQuantity = input.ReceivedQuantity;
            line.RequiredDeliveryDate = input.RequiredDeliveryDate;
            line.LatestCraftCode = input.LatestCraftCode;
            line.ShippingStatus = input.ShippingStatus;
            line.SupplierNotes = input.SupplierNotes;
            line.ActualDeliveryDate = input.ActualDeliveryDate;
            line.UpdateTime = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            if (!_user.IsAdmin && !_user.Roles.Contains("Supervisor"))
                throw new UnauthorizedAccessException();
            if (!await _scope.CanAccessLineAsync(id, ct)) return false;
            var line = await _db.WorkpieceOrderLines!.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (line == null) return false;
            _db.WorkpieceOrderLines!.Remove(line);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> SupplierUpdateAsync(int id, SupplierLineUpdate input, CancellationToken ct = default)
        {
            if (!_user.Roles.Contains("Supplier")) throw new UnauthorizedAccessException();
            if (!await _scope.CanAccessLineAsync(id, ct)) return false;
            var line = await _db.WorkpieceOrderLines!.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (line == null) return false;

            var wantsRepairDates = input.RepairStartedAt.HasValue || input.RepairShippedAt.HasValue;
            if (wantsRepairDates && line.RepairStatus == OrderRepairStatus.None && !line.RepairCreatedAt.HasValue)
                throw new InvalidOperationException("当前无返修任务，请等待监督方发起返修后再填写返修日期");

            if (input.SupplierNotes != null) line.SupplierNotes = input.SupplierNotes;
            if (input.LatestCraftCode != null) line.LatestCraftCode = input.LatestCraftCode;
            if (input.VendorEstimatedDeliveryDate.HasValue)
                line.VendorEstimatedDeliveryDate = input.VendorEstimatedDeliveryDate.Value.Date;
            if (input.ShippedQuantity.HasValue) line.ShippedQuantity = input.ShippedQuantity.Value;

            if (input.ActualDeliveryDate.HasValue)
            {
                line.ActualDeliveryDate = input.ActualDeliveryDate;
                line.ShippingStatus = OrderShippingStatus.Shipped;
            }
            else if (input.ShippingStatus is { } st)
                line.ShippingStatus = st;

            if (input.RepairStartedAt.HasValue) line.RepairStartedAt = input.RepairStartedAt.Value.Date;
            if (input.RepairShippedAt.HasValue) line.RepairShippedAt = input.RepairShippedAt.Value.Date;

            if (line.RepairCreatedAt.HasValue || line.RepairStatus != OrderRepairStatus.None)
            {
                if (line.RepairShippedAt.HasValue)
                    line.RepairStatus = OrderRepairStatus.RepairedShipped;
                else if (line.RepairStartedAt.HasValue)
                    line.RepairStatus = OrderRepairStatus.InProgress;
                else
                    line.RepairStatus = OrderRepairStatus.PendingSupplier;
            }

            line.VendorUpdatedAt = DateTime.UtcNow;
            line.UpdateTime = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
