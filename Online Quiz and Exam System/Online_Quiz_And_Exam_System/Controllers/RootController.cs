using Microsoft.AspNetCore.Mvc;

namespace Online_Quiz_And_Exam_System.Controllers
{
    [ApiController]
    [Route("/")]
    public class RootController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Online Quiz API is running");
        }
    }
}
