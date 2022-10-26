namespace StorApp.Services
{
    public class settings
    {
        

        public settings( IConfiguration configuration)
        {
            mailTo = configuration["mailStrings:mailTo"];
            mailFrom = configuration["mailStrings:mailFrom"];
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
        public string mailTo { get; set; }
        public string mailFrom { get; set; }
        public string DefaultConnection { get; set; }
    }
}
