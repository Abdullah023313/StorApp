using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> GetUser()
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "GivenName")?.Value;

            return Ok(userName??"NALL");
        }
    }
}
