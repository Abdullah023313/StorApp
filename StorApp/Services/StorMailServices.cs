namespace StorApp.Services
{
    namespace StorApi.Services
    {
        public class StorMailServices : IMailServices
        {
            private readonly ILogger<StorMailServices> logger;
            private string mailTo = string.Empty;
            public StorMailServices(ILogger<StorMailServices> logger, IConfiguration configuration)
            {
                mailTo = configuration["mailStrings:mailTo"];
                this.logger = logger;
            }
            public void Send()
            {
                logger.LogInformation($"{mailTo}");
            }
        }
    }
}