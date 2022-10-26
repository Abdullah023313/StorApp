namespace StorApp.Services
{
    namespace StorApi.Services
    {
        public class MailServices : IMailService
        {
            private readonly ILogger<MailServices> logger;
            private string mailTo = string.Empty;
            private string mailFrom = string.Empty;
            public MailServices(ILogger<MailServices> logger, IConfiguration configuration)
            {
                mailTo = configuration["mailStrings:mailTo"];
                mailFrom = configuration["mailStrings:mailFrom"];
                this.logger = logger;
            }

            public async Task SendEmailAsync(string toEmail, string subject, string content)
            {
                //ToDo:
               
            }
        }
    }
}