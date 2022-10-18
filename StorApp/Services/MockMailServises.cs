using StorApp.Model;
using StorApp.Services.StorApi.Services;

namespace StorApp.Services
{
    public class MockMailServises : IMailServices
    {
        private readonly ILogger<MockMailServises> logger;
        private string mailTo = string.Empty;
     
        public MockMailServises(ILogger<MockMailServises> logger, IConfiguration configuration)
        {
            mailTo = configuration["mailStrings:mailTo"];
            this.logger = logger;
        }
        public void Send()
        {
           
            logger.LogInformation($"{mailTo} ");
        }
    }
}
