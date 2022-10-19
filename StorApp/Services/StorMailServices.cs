namespace StorApp.Services
{
    namespace StorApi.Services
    {
        public class StorMailServices : IMailServices
        {
            private readonly ILogger<StorMailServices> logger;
            private string mailTo = string.Empty;
            private string mailFrom = string.Empty;
            public StorMailServices(ILogger<StorMailServices> logger, IConfiguration configuration)
            {
                mailTo = configuration["mailStrings:mailTo"];
                mailFrom = configuration["mailStrings:mailFrom"];
                this.logger = logger;
            }
            public void Send(int productId)
            {

                logger.LogInformation($"The product {productId} has been deleted");
                logger.LogInformation($"The message was sent from {mailFrom} to {mailTo}");
            }
        }
    }
}