using HRMS.Core.Entities;
using HRMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Infrastructure.Repositories
{
    public class LeaveRepository
    {
        private readonly HRMSDbContext _context;
        public LeaveRepository(HRMSDbContext context) => _context = context;

        public async Task<IEnumerable<LeaveRequest>> GetAllAsync()
        {
            return await _context.LeaveRequests.Include(l => l.Employee).ToListAsync();
        }

        public async Task<IEnumerable<LeaveRequest>> GetByEmployeeAsync(int employeeId)
        {
            return await _context.LeaveRequests
                .Where(l => l.EmployeeId == employeeId)
                .Include(l => l.Employee)
                .ToListAsync();
        }

        public async Task<LeaveRequest?> GetByIdAsync(int id)
        {
            return await _context.LeaveRequests.Include(l => l.Employee).FirstOrDefaultAsync(l => l.LeaveRequestId == id);
        }

        public async Task AddAsync(LeaveRequest leave)
        {
            _context.LeaveRequests.Add(leave);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LeaveRequest leave)
        {
            _context.LeaveRequests.Update(leave);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var l = await _context.LeaveRequests.FindAsync(id);
            if (l != null)
            {
                _context.LeaveRequests.Remove(l);
                await _context.SaveChangesAsync();
            }
        }
    }
}
