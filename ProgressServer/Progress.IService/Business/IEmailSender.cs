namespace Progress.IService.Business
{
    public interface IEmailSender
    {
        Task<bool> SendAsync(string subject, string body, string recipientList, CancellationToken ct = default);
    }
}
