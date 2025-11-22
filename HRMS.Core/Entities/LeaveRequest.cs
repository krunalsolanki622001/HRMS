using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Core.Entities
{
    public class LeaveRequest
    {
        public int LeaveRequestId { get; set; }
        public int EmployeeId { get; set; }           // FK to Employee
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; } = "Pending"; // Pending / Approved / Rejected
        public DateTime AppliedOn { get; set; } = DateTime.UtcNow;
        public int? ApprovedBy { get; set; }          // Admin employee id who approved
        public DateTime? ActionedOn { get; set; }

        // Navigation - ignored for JSON
        [System.Text.Json.Serialization.JsonIgnore]
        public Employee? Employee { get; set; }
    }
}
