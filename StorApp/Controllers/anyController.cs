using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StorApp.Services.StorApi.Services;

namespace StorApp.Controllers
{
    [Route("api/any")]
    [ApiController]
    public class anyController : ControllerBase
    {
        [HttpPost("{LName}")]
        //public ActionResult GetResult([FromBody]string FName ,[FromRoute]string LName)
        //{
        //    return Ok($"{LName} {FName}");
        //}
        public ActionResult GetResult([FromQuery] string FName, [FromRoute] string LName, int age ,[FromHeader] string Password ,[FromForm] string city , [FromServices] IMailServices mailServices)
        {
            return Ok($" Hello {LName} {FName} ,Your are age:{age} From {city} , {Password}");
        }

    }
}
