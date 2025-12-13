namespace LeaveManagement.API.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeEmail {  get; set; } = string.Empty;
        public string LeaveType {  get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public string? Filepath { get; set; }
        public string Status { get; set; } = "pending";
        public string? ApprovedBy { get; set; }
        public string? RejectComment { get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.Now;
        public int DaysTaken {  get; set; }

        //Navigation Property
        public virtual Employee? Employee { get; set; }
    }
}
