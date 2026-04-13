namespace Progress.IService.Business
{
    public interface IAlertScanService
    {
        Task RunOnceAsync(CancellationToken ct = default);
    }
}
