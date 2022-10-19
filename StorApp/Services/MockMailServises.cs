using StorApp.Model;
using StorApp.Services.StorApi.Services;

namespace StorApp.Services
{
    public class MockMailServises : IMailServices
    {
        private readonly ILogger<MockMailServises> logger;
        private string mailTo = string.Empty;
        private string mailFrom= string.Empty;

        public MockMailServises(ILogger<MockMailServises> logger, IConfiguration configuration)
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
