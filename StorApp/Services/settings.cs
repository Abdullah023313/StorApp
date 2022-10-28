namespace StorApp.Services
{
    public class Settings
    {


        public Settings(IConfiguration configuration)
        {
            Email = configuration["MailSettings:Email"];
            Issuer = configuration["Authentication:Issuer"];
            Secret = configuration["Authentication:Secret"];
            Audience = configuration["Authentication:Audience"];
            expires = DateTime.UtcNow.AddHours(Convert.ToDouble(configuration["Authentication:DurationInHours"]));
            DefaultConnection = configuration.GetConnectionString("DefaultConnection");
            Host = configuration["MailSettings:Host"];
            Port = Convert.ToInt32(configuration["MailSettings:Port"]);
            Password = configuration["MailSettings:Password"];
            DisplayName = configuration["MailSettings:DisplayName"];
        }

        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
        public DateTime expires { get; set; }
        public string Email { get; set; }
        public string DefaultConnection { get; set; }
        public string Host { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }

        public string DisplayName { get; set; }
    }
}
