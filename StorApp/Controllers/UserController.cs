using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StorApp.Model.UserManager;
using StorApp.Services;

namespace StorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("AddRole")]
        public async Task<ActionResult> AddRole(string rolename)
        {
            if (string.IsNullOrEmpty(rolename))
                return NotFound();
            var result = await _userService.addRole(rolename);

            if (result.IsSuccess)
                return Ok(result); // 200

            return BadRequest(result); // 400
        }

        [HttpPut("updateRole")]
        public async Task<ActionResult> updateRole(string oldRolename, string newRolename)
        {
            if (string.IsNullOrEmpty(oldRolename))
                return NotFound();

            if (string.IsNullOrEmpty(newRolename))
                return NotFound();

            var result = await _userService.updateRole(oldRolename, newRolename);

            if (result.IsSuccess)
                return Ok(result); // 200

            return BadRequest(result); // 400
        }


        [HttpPost("addRoleToUser")]
        public async Task<ActionResult> addRoleToUser(string email, string rolename)
        {
            if (string.IsNullOrEmpty(rolename))
                return NotFound();
            var result = await _userService.addRoleToUser(email, rolename);

            if (result.IsSuccess)
                return Ok(result); // 200

            return BadRequest(result); // 400
        }
    }
}
