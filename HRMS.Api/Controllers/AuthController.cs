using HRMS.Core.DTOs;
using HRMS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly HRMSDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(HRMSDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            var user = _context.Employees.SingleOrDefault(x => x.EmployeeCode == request.EmployeeCode);

            if (user == null) return Unauthorized("Invalid Employee Code");

            if (user.PasswordHash != request.Password) return Unauthorized("Invalid Password");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.EmployeeId.ToString()),
                new Claim(ClaimTypes.Name, user.EmployeeCode),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: creds
            );

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserName = user.Name,
                Role = user.Role
            });
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-stats")]
        public async Task<IActionResult> GetAdminStats()
        {
            var totalEmployees = await _context.Employees.CountAsync();
            var pendingLeaves = await _context.LeaveRequests.Where(x => x.Status == "Pending").CountAsync();
            var approvedLeaves = await _context.LeaveRequests.Where(x => x.Status == "Approved").CountAsync();

            return Ok(new
            {
                totalEmployees,
                pendingLeaves,
                approvedLeaves
            });
        }
        [Authorize(Roles = "Employee")]
        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (idClaim == null)
                return BadRequest("NameIdentifier claim missing from token");

            int empId = int.Parse(idClaim.Value);

            var employee = await _context.Employees
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.EmployeeId == empId);

            if (employee == null)
                return NotFound();

            return Ok(employee);

        }
    }


}
