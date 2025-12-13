using LeaveManagement.API.Models;

namespace LeaveManagement.API.Models
{
    public class Employee
    {
        public String EmployeeId { get; set; } = string.Empty;
        public String Name { get; set; } = string.Empty;
        public String Email { get; set; } = string.Empty;
        public String Department { get; set; } = string.Empty;
        public String Role { get; set; } = string.Empty;
        public String ManagerId { get; set; } = string.Empty;

        //Navigation properties
        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public virtual ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
        public virtual ICollection<Notification> Notifications { get; set; }= new List<Notification>();

    }
}
