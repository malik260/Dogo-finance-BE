using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using DogoFinance.Integration.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using DogoFinance.DataAccess.Layer.Models.Entities;
using Microsoft.EntityFrameworkCore;

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

        public async Task<bool> SendEmail(string to, string subject, string body, IEnumerable<LinkedResource>? resources = null)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "localhost";
                var port = int.Parse(_configuration["EmailSettings:Port"] ?? "25");
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];
                var fromAddress = _configuration["EmailSettings:FromAddress"] ?? "noreply@dogofinance.com";
                var displayName = _configuration["EmailSettings:DisplayName"] ?? "DogoFinance";
                try
                {
                    using var context = new DogoDbContext();
                    var profile = await context.Set<TblCompanyProfile>().FirstOrDefaultAsync();
                    if (profile != null && !string.IsNullOrEmpty(profile.CompanyName))
                    {
                        displayName = profile.CompanyName;
                    }
                }
                catch { /* fallback to config if database isn't ready or fails */ }
                var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");

                using var mail = new MailMessage
                {
                    From = new MailAddress(fromAddress, displayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                if (resources != null && resources.Any())
                {
                    var alternateView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                    foreach (var resource in resources)
                    {
                        alternateView.LinkedResources.Add(resource);
                    }
                    mail.AlternateViews.Add(alternateView);
                }

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

        private async Task InjectCompanyProfilePlaceholders(Dictionary<string, string> placeholders)
        {
            try
            {
                using var context = new DogoDbContext();
                var profile = await context.Set<TblCompanyProfile>().FirstOrDefaultAsync();
                if (profile != null)
                {
                    placeholders["CompanyName"] = profile.CompanyName;
                    placeholders["CompanyAddress"] = profile.Address;
                    placeholders["CompanyPhone"] = profile.PhoneNumber;
                    placeholders["CompanyEmail"] = profile.Email;
                    placeholders["CompanyRcNumber"] = profile.RcNumber;
                    placeholders["CompanyAccountNumber"] = profile.AccountNumber ?? "";
                    placeholders["CompanyXLink"] = profile.XLink ?? "";
                    placeholders["CompanyFacebookLink"] = profile.FacebookLink ?? "";
                }
                else
                {
                    placeholders["CompanyName"] = "DogoFinance";
                    placeholders["CompanyAddress"] = "Tesmot House, 3 Abdulrahman Okene Close, Victoria Island, Lagos";
                    placeholders["CompanyPhone"] = "+234 800 000 0000";
                    placeholders["CompanyEmail"] = "info@dogofinance.com";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load company profile for email templates. Using defaults.");
                placeholders["CompanyName"] = "DogoFinance";
                placeholders["CompanyAddress"] = "Tesmot House, 3 Abdulrahman Okene Close, Victoria Island, Lagos";
                placeholders["CompanyPhone"] = "+234 800 000 0000";
                placeholders["CompanyEmail"] = "info@dogofinance.com";
            }
        }

        public async Task<bool> SendTemplateEmail(string to, string subject, string templateName, Dictionary<string, string> placeholders)
        {
            try
            {
                // Inject Company Profile Placeholders
                await InjectCompanyProfilePlaceholders(placeholders);

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
                //foreach (var placeholder in placeholders)
                //{
                //    var value = WebUtility.HtmlEncode(placeholder.Value);
                //    body = body.Replace($"{{{{{placeholder.Key}}}}}", value);
                //}

                foreach (var placeholder in placeholders)
                {
                    var key = placeholder.Key;
                    var rawValue = placeholder.Value ?? string.Empty;

                    bool isLink = key.ToLower().Contains("link") || key.ToLower().Contains("url");

                    var value = isLink
                        ? rawValue
                        : WebUtility.HtmlEncode(rawValue);

                    body = body.Replace($"{{{{{key}}}}}", value);
                }

                if (body.Contains("{{LogoUrl}}"))
                {
                    body = body.Replace("{{LogoUrl}}", "cid:logo");
                }

                var resources = new List<LinkedResource>();
                
                // If the body contains cid:logo, try to attach the logo
                if (body.Contains("cid:logo"))
                {
                    var logoPath = Path.Combine(_env.ContentRootPath, "Images", "DOGO.jpg.webp");
                    if (!File.Exists(logoPath))
                    {
                        // Try fallback path
                        logoPath = Path.Combine(_env.ContentRootPath, "..", "DogoFinance.Api", "Images", "DOGO.jpg.webp");
                    }

                    if (File.Exists(logoPath))
                    {
                        var logoResource = new LinkedResource(logoPath, "image/webp") { ContentId = "logo" };
                        resources.Add(logoResource);
                    }
                    else
                    {
                        _logger.LogWarning("Logo for CID embedding not found at {Path}", logoPath);
                    }
                }

                return await SendEmail(to, subject, body, resources);
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
