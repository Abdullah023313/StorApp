using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StorApp.Model;
using StorApp.Services.StorApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StorApp.Controllers
{
    [Route("api/v{version:apiVersion}/Authentication")]
    [ApiController]
    [ApiVersion("2.0")]
 
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        /*[FromQuery] string FName, [FromRoute] string LName, int age ,[FromHeader] string Password ,[FromForm] string city , [FromServices] IMailServices mailServices*/

        /// <summary>
        ///  
        /// </summary>
        /// <param name="user">is Class Include UserName and Password</param>
        /// <returns>return myToken</returns>
        [HttpPost]
        public ActionResult RegisterAsync([FromForm] UserRegister user)
        {
            if (user == null)
                return Unauthorized();

            var claims = new List<Claim>()
            {
                new Claim("Sub","1"),
                new Claim("GivenName","Abdullah"),
            };

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Authentication:Secret"]));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: configuration["Authentication:Issuer"],
                audience: configuration["Authentication:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(double.Parse(configuration["Authentication:DurationInHours"])),
                signingCredentials: signingCredentials);

            var myToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return Ok(myToken);
        } 
    }
}
