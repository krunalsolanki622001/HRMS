using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HRMS.Core.Entities
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }   // NEW
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
        public DateTime JoiningDate { get; set; }
        public int DepartmentId { get; set; }
        public string Role { get; set; } // Admin / Employee

        public string PasswordHash { get; set; }  // NEW
        
        public Department? Department { get; set; } 
    }
}
