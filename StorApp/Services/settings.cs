namespace StorApp.Services
{
    public class Settings
    {
        

        public Settings( IConfiguration configuration)
        {
            Email = configuration["mailStrings:Email"];
            Issuer = configuration["Authentication:Issuer"] ;
            Secret= configuration["Authentication:Secret"];
            Audience = configuration["Authentication:Audience"];
            expires = DateTime.UtcNow.AddHours(Convert.ToDouble(configuration["Authentication:DurationInHours"]));
            DefaultConnection = configuration.GetConnectionString("DefaultConnection");
        }

        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
        public DateTime expires { get; set; }
        public string Email { get; set; }
        public string DefaultConnection { get; set; }
    }
}
