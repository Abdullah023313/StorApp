using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;

namespace StorApp.Services
{
    namespace StorApi.Services
    {
        public class MailServices : IMailService
        {
            private readonly ILogger<MailServices> _logger;
            private readonly Settings _settings;
            public MailServices(ILogger<MailServices> logger,Settings settings)
            {
                
                _logger = logger;
                _settings = settings;
            }

            public async Task SendEmailAsync(string toEmail, string subject, string content)
            {
                //ToDo:
                var client = new SendGridClient(_settings.Secret);
                var from = new EmailAddress(_settings.Email, "JWT Auth Demo");
                var to = new EmailAddress(toEmail);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
                var response = await client.SendEmailAsync(msg);
                _logger.LogInformation($"The message was sent from {_settings.Email} to {toEmail}");
            }
        }
    }
}