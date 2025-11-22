using HRMS.Core.Entities;
using HRMS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveController : ControllerBase
    {
        private readonly LeaveRepository _repo;
        private readonly EmployeeRepository _empRepo;

        public LeaveController(LeaveRepository repo, EmployeeRepository empRepo)
        {
            _repo = repo;
            _empRepo = empRepo;
        }

        // Admin: get all leave requests
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var all = await _repo.GetAllAsync();
            var result = all.Select(l => new {
                l.LeaveRequestId,
                l.EmployeeId,
                EmployeeName = l.Employee?.Name,
                l.StartDate,
                l.EndDate,
                l.Reason,
                l.Status,
                l.AppliedOn,
                l.ApprovedBy,
                l.ActionedOn
            });
            return Ok(result);
        }

        // Employee: get my leaves
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMine()
        {
            var empId = await GetCurrentEmployeeId();
            if (empId == null) return Unauthorized();

            var mine = await _repo.GetByEmployeeAsync(empId.Value);
            var result = mine.Select(l => new {
                l.LeaveRequestId,
                l.EmployeeId,
                EmployeeName = l.Employee?.Name,
                l.StartDate,
                l.EndDate,
                l.Reason,
                l.Status,
                l.AppliedOn,
                l.ApprovedBy,
                l.ActionedOn
            });
            return Ok(result);
        }

        // Apply
        [HttpPost("apply")]
        [Authorize]
        public async Task<IActionResult> Apply([FromBody] LeaveRequest request)
        {
            var empId = await GetCurrentEmployeeId();
            if (empId == null) return Unauthorized();

            if (request == null || request.StartDate == default || request.EndDate == default)
                return BadRequest("Invalid dates");

            request.EmployeeId = empId.Value;
            request.Status = "Pending";
            request.AppliedOn = DateTime.UtcNow;
            await _repo.AddAsync(request);
            return Ok(request);
        }

        // Admin approve
        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var leave = await _repo.GetByIdAsync(id);
            if (leave == null) return NotFound();

            var approverId = await GetCurrentEmployeeId();
            leave.Status = "Approved";
            leave.ApprovedBy = approverId;
            leave.ActionedOn = DateTime.UtcNow;
            await _repo.UpdateAsync(leave);
            return Ok(leave);
        }

        // Admin reject
        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var leave = await _repo.GetByIdAsync(id);
            if (leave == null) return NotFound();

            var approverId = await GetCurrentEmployeeId();
            leave.Status = "Rejected";
            leave.ApprovedBy = approverId;
            leave.ActionedOn = DateTime.UtcNow;
            await _repo.UpdateAsync(leave);
            return Ok(leave);
        }

        // Helper — get current employee id from JWT (assumes ClaimTypes.Name = EmployeeCode)
        private async Task<int?> GetCurrentEmployeeId()
        {
            var code = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(code)) return null;
            var emp = (await _empRepo.GetAllAsync()).FirstOrDefault(e => e.EmployeeCode == code);
            return emp?.EmployeeId;
        }
    }
}
