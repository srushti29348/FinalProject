using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_Quiz_And_Exam_System.Models;
using Online_Quiz_API.DAL;

namespace Online_Quiz_And_Exam_System.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/result")]
    public class ResultController : ControllerBase
    {
        private readonly ResultDAL _dal;

        public ResultController(ResultDAL dal)
        {
            _dal = dal;
        }

        [HttpPost]
        public IActionResult Save([FromBody] TestResult r)
        {
            if (string.IsNullOrEmpty(r.TestType))
                return BadRequest("TestType not received");

            _dal.Save(r);
            return Ok();
        }

        [HttpGet("attempts/{userId}")]
        public IActionResult GetAttempts(int userId)
        {
            return Ok(_dal.GetAttemptSummary(userId));
        }

        [HttpGet("latest/{userId}")]
        public IActionResult Latest(int userId)
        {
            var result = _dal.GetLatestResult(userId);

            if (result == null)
            {
                return Ok(new
                {
                    moduleName = "N/A",
                    score = 0,
                    attempted = 0,
                    unattempted = 0,
                    totalTests = 0,
                    practiceTests = 0,
                    mockTests = 0,
                    bestScore = 0
                });
            }

            return Ok(result);
        }



        [HttpGet("check-mock")]
        public IActionResult CheckMockAttempt(
            int userId,
            int moduleId,
            int mockNo)
        {
            bool attempted = _dal.HasAttemptedMock(userId, moduleId, mockNo);
            return Ok(new { attempted });
        }

    }
}
