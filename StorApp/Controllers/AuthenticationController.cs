using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using StorApp.Model;
using StorApp.Model.Dtos;
using StorApp.Model.UserManager;
using StorApp.Services;
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
        private readonly Settings _settings;
        private readonly IUserService _userService;
        private readonly IMailService _mailService;

        public AuthenticationController(IUserService userService, IMailService mailService, Settings settings)
        {
            _userService = userService;
            _mailService = mailService;
            _settings = settings;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<ActionResult> LoginAsync(Login model)
        {
            var result = await _userService.LoginUserAsync(model);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="model">is model Include Email , Password and ConfirmPassword</param>
        /// <returns>return UserResponse</returns>
        [HttpPost("Register")]
        public async Task<ActionResult> RegisterAsync(Register model)
        {
            var result = await _userService.RegisterUserAsync(model);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<ActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();

            var result = await _userService.ConfirmEmailAsync(userId, token);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm]ResetPassword model)
        {
            var result = await _userService.ResetPasswordAsync(model);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }
    }
}