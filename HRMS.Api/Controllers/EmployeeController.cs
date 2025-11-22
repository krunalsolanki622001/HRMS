using HRMS.Core.Entities;
using HRMS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BC = BCrypt.Net.BCrypt;
namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeRepository _repo;

        public object BCrypt { get; private set; }

        public EmployeeController(EmployeeRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _repo.GetAllAsync();

            // return with DepartmentName for React list
            var result = employees.Select(e => new
            {
                e.EmployeeId,
                e.EmployeeCode,
                e.Name,
                e.Email,
                e.Phone,
                e.Address,
                e.Gender,
                e.DOB,
                e.JoiningDate,
                DepartmentName = e.Department?.DepartmentName,
                e.Role
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var emp = await _repo.GetByIdAsync(id);
            if (emp == null) return NotFound();

            var result = new
            {
                emp.EmployeeId,
                emp.EmployeeCode,
                emp.Name,
                emp.Email,
                emp.Phone,
                emp.Address,
                emp.Gender,
                emp.DOB,
                emp.JoiningDate,
                emp.DepartmentId,
                DepartmentName = emp.Department?.DepartmentName,
                emp.Role
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Employee employee)
        {
            // Hash password before save
            //if (!string.IsNullOrEmpty(employee.PasswordHash))
            //{
            //    employee.PasswordHash = BC.HashPassword(employee.PasswordHash);
            //}

            await _repo.AddAsync(employee);
            return Ok("Employee added successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Employee employee)
        {
            if (id != employee.EmployeeId)
                return BadRequest("Invalid Employee ID");

            //if (!string.IsNullOrEmpty(employee.PasswordHash))
            //{
            //    employee.PasswordHash = BC.HashPassword(employee.PasswordHash);
            //}

            await _repo.UpdateAsync(employee);
            return Ok("Employee updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repo.DeleteAsync(id);
            return Ok("Employee deleted successfully.");
        }

       

    }
}
