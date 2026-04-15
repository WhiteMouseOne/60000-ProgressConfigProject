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

        /// <summary>校验 LatestCraftCode 对应工艺存在；若指定配方则须在该配方步序中。</summary>
        private async Task ValidateLatestCraftInRecipeAsync(int? craftRecipeId, int? latestCraftCode, CancellationToken ct)
        {
            if (latestCraftCode is not { } code) return;
            var craft = await _db.Crafts!.AsNoTracking().FirstOrDefaultAsync(c => c.Code == code, ct);
            if (craft == null) throw new InvalidOperationException($"工艺编码不存在: {code}");
            if (craftRecipeId is not { } rid) return;
            var inRecipe = await _db.CraftRecipeSteps!.AnyAsync(s => s.CraftRecipeId == rid && s.CraftId == craft.Id, ct);
            if (!inRecipe) throw new InvalidOperationException("所选「最新工艺」不属于当前工艺配方步序");
        }

        private async Task FillLatestCraftNamesAsync(IList<OrderLineDto> items, CancellationToken ct)
        {
            if (items.Count == 0) return;
            var codes = items.Where(i => i.LatestCraftCode.HasValue).Select(i => i.LatestCraftCode!.Value).Distinct().ToList();
            if (codes.Count == 0) return;
            var crafts = await _db.Crafts!.AsNoTracking()
                .Where(c => codes.Contains(c.Code))
                .ToDictionaryAsync(c => c.Code, c => new { c.Id, c.Name }, ct);
            var recipeIds = items.Where(i => i.CraftRecipeId != null).Select(i => i.CraftRecipeId!.Value).Distinct().ToList();
            HashSet<(int RecipeId, int CraftId)>? stepSet = null;
            if (recipeIds.Count > 0)
            {
                var steps = await _db.CraftRecipeSteps!.AsNoTracking()
                    .Where(s => recipeIds.Contains(s.CraftRecipeId))
                    .Select(s => new { s.CraftRecipeId, s.CraftId })
                    .ToListAsync(ct);
                stepSet = steps.Select(s => (s.CraftRecipeId, s.CraftId)).ToHashSet();
            }

            foreach (var item in items)
            {
                if (!item.LatestCraftCode.HasValue || !crafts.TryGetValue(item.LatestCraftCode.Value, out var cr))
                {
                    item.LatestCraftName = null;
                    continue;
                }

                if (item.CraftRecipeId == null)
                {
                    item.LatestCraftName = cr.Name;
                    continue;
                }

                if (stepSet != null && stepSet.Contains((item.CraftRecipeId.Value, cr.Id)))
                    item.LatestCraftName = cr.Name;
                else
                    item.LatestCraftName = null;
            }
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
                    CraftRecipeId = x.CraftRecipeId,
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
            await FillLatestCraftNamesAsync(items, ct);
            return new PagedResult<OrderLineDto> { Items = items, Total = total };
        }

        public async Task<OrderLineDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            if (!await _scope.CanAccessLineAsync(id, ct)) return null;
            var x = await _db.WorkpieceOrderLines!.AsNoTracking().Include(s => s.Supplier)
                .FirstOrDefaultAsync(l => l.Id == id, ct);
            if (x == null) return null;
            var dto = new OrderLineDto
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
                CraftRecipeId = x.CraftRecipeId,
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
            await FillLatestCraftNamesAsync(new List<OrderLineDto> { dto }, ct);
            return dto;
        }

        public async Task<int> CreateAsync(OrderLineCreateOrEdit input, CancellationToken ct = default)
        {
            if (!_user.IsAdmin && !_user.Roles.Contains("Supervisor"))
                throw new UnauthorizedAccessException();
            ValidatePo(input.PoNumber);
            if (!await _db.Suppliers!.AnyAsync(s => s.Id == input.SupplierId, ct))
                throw new InvalidOperationException("供应商不存在");
            if (input.CraftRecipeId is { } newRid && !await _db.CraftRecipes!.AnyAsync(r => r.Id == newRid, ct))
                throw new InvalidOperationException("工艺配方不存在");
            await ValidateLatestCraftInRecipeAsync(input.CraftRecipeId, input.LatestCraftCode, ct);
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
                CraftRecipeId = input.CraftRecipeId,
                LatestCraftCode = input.LatestCraftCode,
                ShippedQuantity = null,
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
            if (input.CraftRecipeId is { } rid && !await _db.CraftRecipes!.AnyAsync(r => r.Id == rid, ct))
                throw new InvalidOperationException("工艺配方不存在");
            await ValidateLatestCraftInRecipeAsync(input.CraftRecipeId, input.LatestCraftCode, ct);
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
            line.CraftRecipeId = input.CraftRecipeId;
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
            if (_user.IsSupplierAccount != 1) throw new UnauthorizedAccessException();
            if (!await _scope.CanAccessLineAsync(id, ct)) return false;
            var line = await _db.WorkpieceOrderLines!.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (line == null) return false;

            var wantsRepairDates = input.RepairStartedAt.HasValue || input.RepairShippedAt.HasValue;
            if (wantsRepairDates && line.RepairStatus == OrderRepairStatus.None && !line.RepairCreatedAt.HasValue)
                throw new InvalidOperationException("当前无返修任务，请等待监督方发起返修后再填写返修日期");

            if (input.SupplierNotes != null) line.SupplierNotes = input.SupplierNotes;
            if (input.LatestCraftCode.HasValue)
            {
                await ValidateLatestCraftInRecipeAsync(line.CraftRecipeId, input.LatestCraftCode, ct);
                line.LatestCraftCode = input.LatestCraftCode;
            }
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
