using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
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
            public MailServices(ILogger<MailServices> logger, Settings settings)
            {

                _logger = logger;
                _settings = settings;
            }

            public async Task SendEmailAsync(string mailTo, string subject, string body, string displayName, IList<IFormFile> attachments = null)
            {
                try
                {
                    var email = new MimeMessage
                    {
                        Sender = MailboxAddress.Parse(_settings.Email),
                        Subject = subject
                    };

                    email.To.Add(MailboxAddress.Parse(mailTo));

                    var builder = new BodyBuilder();

                    if (attachments != null)
                    {
                        byte[] fileBytes;
                        foreach (var file in attachments)
                        {
                            if (file.Length > 0)
                            {
                                using var ms = new MemoryStream();
                                file.CopyTo(ms);
                                fileBytes = ms.ToArray();

                                builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                            }
                        }
                    }

                    builder.HtmlBody = body;
                    email.Body = builder.ToMessageBody();
                    email.From.Add(new MailboxAddress(displayName, _settings.Email));

                    using var smtp = new SmtpClient();
                    smtp.Connect(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
                    smtp.Authenticate(_settings.Email, _settings.Password);
                    await smtp.SendAsync(email);
                    smtp.Disconnect(true);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Error in SendEmailAsync {ex.Message}");
                }
            }
        }
    }
}