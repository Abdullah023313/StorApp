using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StorApp.Services.StorApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StorApp.Controllers
{
    [Route("api/Authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        /*[FromQuery] string FName, [FromRoute] string LName, int age ,[FromHeader] string Password ,[FromForm] string city , [FromServices] IMailServices mailServices*/
        [HttpGet]
        public ActionResult RegisterAsync()
        {


            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.GivenName,"Abdullah"),
                new Claim(JwtRegisteredClaimNames.Email, ""),
             
            };

            var Secret = configuration["Authentication:Secret"];
            var LengthSecret = Secret.Length;
            var EncodingSecret = Encoding.UTF8.GetBytes(Secret);
            var symmetricSecurityKey = new SymmetricSecurityKey(EncodingSecret);
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: configuration["Authentication:Issuer"],
                audience: configuration["Authentication:Audience"],
                claims: null,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(double.Parse(configuration["Authentication:DurationInHours"])),
                signingCredentials: signingCredentials);

            var muToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return Ok(muToken);
        }
    }
}
