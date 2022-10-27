using SendGrid.Helpers.Mail;
using SendGrid;
using StorApp.Model;
using StorApp.Services.StorApi.Services;
using StorApp.Extensions;
using System.Net.Mail;
using System.Net;

namespace StorApp.Services
{
    public class MockMailServises : IMailService
    {
        private readonly ILogger<MockMailServises> _logger;
        private readonly Settings _settings;

        public MockMailServises(ILogger<MockMailServises> logger, Settings settings)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            //ToDo:

            try
            {
                var fromMail = "<Add Your Email>";
                var fromPassword = "<Add Your Password>";

                var message = new MailMessage();

                message.From = new MailAddress(_settings.Email);
                message.Subject = subject;
                message.To.Add(_settings.Email);
                message.Body = $"<html><body>{content}</body></html>";
                message.IsBodyHtml = true;

                var smtpClient = new SmtpClient(_settings.Email)
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true
                };

                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
                _logger.LogWarning(content);
            }

        }

    }
}
