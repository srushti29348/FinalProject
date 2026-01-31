using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_Quiz_And_Exam_System.DAL;

namespace Online_Quiz_And_Exam_System.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/mock")]
    public class MockController : ControllerBase
    {
        private readonly MockDAL _dal;
        public MockController(MockDAL dal) => _dal = dal;

        [HttpGet("{moduleId}/{mockNumber}")]
        public IActionResult GetMock(int moduleId, int mockNumber)
        {
            if (mockNumber < 1 || mockNumber > 5)
                return BadRequest("Invalid mock number");

            var questions = _dal.GetMockQuestions(moduleId, mockNumber);
            return Ok(questions);
        }
    }

}
