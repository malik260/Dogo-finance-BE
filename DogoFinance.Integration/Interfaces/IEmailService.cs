using System.Net.Mail;

namespace DogoFinance.Integration.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmail(string to, string subject, string body, IEnumerable<LinkedResource>? resources = null);
        Task<bool> SendTemplateEmail(string to, string subject, string templateName, Dictionary<string, string> placeholders);
        Task<bool> SendEmailWithAttachment(string to, string subject, string body, string attachmentPath);
    }
}
