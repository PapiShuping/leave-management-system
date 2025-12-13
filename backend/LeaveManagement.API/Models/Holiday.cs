namespace LeaveManagement.API.Models
{
    public class Holiday
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsWeekend { get; set; } = false;
    }
}
