using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_Quiz_API.DAL;

namespace Online_Quiz_And_Exam_System.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/modules")]
    public class ModuleController : ControllerBase
    {
        private readonly ModuleDAL _dal;

        public ModuleController(ModuleDAL dal)
        {
            _dal = dal;
        }

        [HttpGet]
        public IActionResult GetModules()
        {
            var modules = _dal.GetAllModules();
            return Ok(modules);
        }
    }
}
