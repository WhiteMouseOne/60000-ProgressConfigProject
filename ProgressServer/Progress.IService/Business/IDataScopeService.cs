using Progress.Model.Entitys;

namespace Progress.IService.Business
{
    public interface IDataScopeService
    {
        IQueryable<WorkpieceOrderLine> FilteredOrderLines();
        Task<bool> CanAccessLineAsync(int lineId, CancellationToken ct = default);
    }
}
