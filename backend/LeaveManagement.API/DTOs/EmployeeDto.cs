namespace LeaveManagement.API.DTOs
{
    public class EmployeeDto
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ManagerId { get; set; }
    }

    public class EmployeeWithBalancesDto
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<LeaveBalanceDto> LeaveBalances { get; set; } = new();
    }

    public class LeaveBalanceDto
    {
        public string LeaveType { get; set; } = string.Empty;
        public int DaysAllocated { get; set; }
        public int DaysUsed { get; set; }
        public int DaysLeft { get; set; }
    }
}