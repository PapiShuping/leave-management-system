//Dev
//Kabo Shuping
//LinkedIn:
//GitHub:
//email:shupingkabo04@gmail.com
namespace LeaveManagement.API.Models
{
    public class LeaveBalance
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string LeaveType { get; set; }
        public int DaysAllocated { get; set; }
        public int DaysUsed { get; set; } = 0;
        public int DaysLeft { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        //Navigation Property
        public virtual Employee? Employee { get; set; }
    }
}
