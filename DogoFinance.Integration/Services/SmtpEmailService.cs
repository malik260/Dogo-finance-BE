using System.Net;
using System.Net.Mail;
using DogoFinance.Integration.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace DogoFinance.Integration.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailService> _logger;
        private readonly IWebHostEnvironment _env;

        public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _logger = logger;
            _env = env;
        }

        public async Task<bool> SendEmail(string to, string subject, string body)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "localhost";
                var port = int.Parse(_configuration["EmailSettings:Port"] ?? "25");
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];
                var fromAddress = _configuration["EmailSettings:FromAddress"] ?? "noreply@dogofinance.com";
                var displayName = _configuration["EmailSettings:DisplayName"] ?? "DogoFinance";
                var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");

                using var mail = new MailMessage
                {
                    From = new MailAddress(fromAddress, displayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(to);

                using var smtpClient = new SmtpClient(smtpServer, port);
                smtpClient.EnableSsl = enableSsl;
                if (!string.IsNullOrEmpty(username))
                {
                    smtpClient.Credentials = new NetworkCredential(username, password);
                }

                await smtpClient.SendMailAsync(mail);
                _logger.LogInformation("Email sent successfully using {SmtpServer} to {To}", smtpServer, to);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email via SMTP to {To}", to);
                return false;
            }
        }

        public async Task<bool> SendTemplateEmail(string to, string subject, string templateName, Dictionary<string, string> placeholders)
        {
            try
            {
                // Find and read template
                var templatePath = Path.Combine(_env.ContentRootPath, "..", "DogoFinance.Integration", "EmailTemplates", $"{templateName}.html");
                if (!File.Exists(templatePath))
                {
                    // Fallback for production or different project structure
                    templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates", $"{templateName}.html");
                }

                if (!File.Exists(templatePath))
                {
                     _logger.LogError("Email template not found: {Path}", templatePath);
                     return false;
                }

                var body = await File.ReadAllTextAsync(templatePath);

                // Replace placeholders
                foreach (var placeholder in placeholders)
                {
                    body = body.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
                }

                return await SendEmail(to, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending templated email {Template} to {To}", templateName, to);
                return false;
            }
        }

        public Task<bool> SendEmailWithAttachment(string to, string subject, string body, string attachmentPath)
        {
            throw new NotImplementedException();
        }
    }
}
