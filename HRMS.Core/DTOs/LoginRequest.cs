using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Core.DTOs
{
    public class LoginRequest
    {
        public string EmployeeCode { get; set; }
        public string Password { get; set; }
    }
}
