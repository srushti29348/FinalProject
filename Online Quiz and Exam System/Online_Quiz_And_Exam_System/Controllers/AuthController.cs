using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Online_Quiz_And_Exam_System.Models;
using Online_Quiz_API.DAL;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Online_Quiz_And_Exam_System.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserDAL _dal;
        private readonly IConfiguration _config;

        public AuthController(UserDAL dal, IConfiguration config)
        {
            _dal = dal;
            _config = config;
        }

        // ================= LOGIN =================
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] User u)
        {
            if (string.IsNullOrEmpty(u.Email) || string.IsNullOrEmpty(u.Password))
                return BadRequest("Email and password are required");

            if (!u.Email.EndsWith("@gmail.com"))
                return BadRequest("Invalid email format");

            var user = _dal.ValidateUser(u.Email, u.Password);

            if (user == null)
                return Unauthorized("Invalid email or password");

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                user,
                token
            });
        }

        // ================= REGISTER =================
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] User u)
        {
            if (string.IsNullOrEmpty(u.Email) || !u.Email.EndsWith("@gmail.com"))
                return BadRequest("Email must be a valid @gmail.com address");

            if (!IsStrongPassword(u.Password))
                return BadRequest("Password does not meet complexity requirements");

            _dal.Register(u);
            return Ok("User registered successfully");
        }

        // ================= GOOGLE LOGIN =================
        [AllowAnonymous]
        [HttpPost("google")]
        public IActionResult GoogleLogin([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Email))
                return BadRequest("Invalid Google user data");

            var existingUser = _dal.GetUserByEmail(user.Email);

            if (existingUser == null)
            {
                _dal.RegisterGoogleUser(user);
                existingUser = _dal.GetUserByEmail(user.Email);
            }

            if (existingUser == null)
                return StatusCode(500, "Google user creation failed");

            var token = GenerateJwtToken(existingUser);

            return Ok(new
            {
                user = existingUser,
                token
            });
        }


        // ================= JWT GENERATOR =================
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName ?? "")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool IsStrongPassword(string password)
        {
            var regex = new System.Text.RegularExpressions.Regex(
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#^()_+\-=])[A-Za-z\d@$!%*?&#^()_+\-=]{8,}$"
            );

            return regex.IsMatch(password);
        }
    }
}
