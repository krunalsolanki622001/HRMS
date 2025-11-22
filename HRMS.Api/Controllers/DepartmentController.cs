using HRMS.Core.Entities;
using HRMS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly DepartmentRepository _repo;

        public DepartmentController(DepartmentRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var dept = await _repo.GetByIdAsync(id);
            if (dept == null) return NotFound();
            return Ok(dept);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Department department)
        {
            await _repo.AddAsync(department);
            return Ok("Department added successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Department department)
        {
            if (id != department.DepartmentId) return BadRequest();
            await _repo.UpdateAsync(department);
            return Ok("Department updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repo.DeleteAsync(id);
            return Ok("Department deleted successfully.");
        }
    }
}
