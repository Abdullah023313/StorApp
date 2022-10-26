using AspNetIdentityDemo.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StorApp.Model;
using StorApp.Model.Dtos;
using StorApp.Services;
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
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IMailService _mailService;

        public AuthenticationController(IUserService userService, IMailService mailService, IConfiguration configuration)
        {
            _userService = userService;
            _mailService = mailService;
            _configuration = configuration;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="model">is model Include Email , Password and ConfirmPassword</param>
        /// <returns>return UserManagerResponse</returns>
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] Register model)
        {
                var result = await _userService.RegisterUserAsync(model);

                if (result.IsSuccess)
                    return Ok(result); 

                return BadRequest(result);
        }
    }
}
