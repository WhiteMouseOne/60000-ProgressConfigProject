using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Progress.Common;
using Progress.IService.Business;

namespace Progress.Service.Business
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailOptions _o;
        private readonly ILogger<EmailSender> _log;

        public EmailSender(IOptions<EmailOptions> opt, ILogger<EmailSender> log)
        {
            _o = opt.Value;
            _log = log;
        }

        public async Task<bool> SendAsync(string subject, string body, string recipientList, CancellationToken ct = default)
        {
            var recipients = recipientList.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(x => x.Length > 0).ToArray();
            if (recipients.Length == 0)
            {
                _log.LogWarning("Email skipped: no recipients for subject {Subject}", subject);
                return false;
            }

            try
            {
                using var client = new SmtpClient(_o.SmtpHost, _o.SmtpPort)
                {
                    EnableSsl = _o.UseSsl,
                    Credentials = string.IsNullOrEmpty(_o.User)
                        ? CredentialCache.DefaultNetworkCredentials
                        : new NetworkCredential(_o.User, _o.Password)
                };
                using var msg = new MailMessage
                {
                    From = new MailAddress(_o.From),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };
                foreach (var r in recipients) msg.To.Add(r);
                await client.SendMailAsync(msg, ct);
                _log.LogInformation("Email sent: {Subject} -> {Recipients}", subject, recipientList);
                return true;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Email failed: {Subject}", subject);
                return false;
            }
        }
    }
}
