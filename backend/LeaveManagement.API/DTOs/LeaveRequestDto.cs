namespace LeaveManagement.API.DTOs
{
    // For submitting new leave request
    public class CreateLeaveRequestDto
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string LeaveType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
        public string? FilePath { get; set; }
    }

    // For returning leave request data
    public class LeaveRequestDto
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeEmail { get; set; } = string.Empty;
        public string LeaveType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
        public string? FilePath { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ApprovedBy { get; set; }
        public string? RejectComment { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? DaysTaken { get; set; }
    }

    // For approving leave request
    public class ApproveLeaveRequestDto
    {
        public string ApprovedBy { get; set; } = string.Empty;
    }

    // For rejecting leave request
    public class RejectLeaveRequestDto
    {
        public string RejectedBy { get; set; } = string.Empty;
        public string RejectComment { get; set; } = string.Empty;
    }
}