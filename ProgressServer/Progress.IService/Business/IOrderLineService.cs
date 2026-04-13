using Progress.Model.Dto.Order;

namespace Progress.IService.Business
{
    public interface IOrderLineService
    {
        Task<PagedResult<OrderLineDto>> QueryAsync(OrderLineQuery query, CancellationToken ct = default);
        Task<OrderLineDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<int> CreateAsync(OrderLineCreateOrEdit input, CancellationToken ct = default);
        Task<bool> UpdateAsync(OrderLineCreateOrEdit input, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> SupplierUpdateAsync(int id, SupplierLineUpdate input, CancellationToken ct = default);
    }
}
