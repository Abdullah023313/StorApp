using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StorApp.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        [HttpGet("fileId")]
        public ActionResult GetFile()
        {
            var path = "TextFile.txt";
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }
            var file = System.IO.File.ReadAllBytes(path);

            return File(file, "text/plain", Path.GetFileName(path));


        }
    }
}
