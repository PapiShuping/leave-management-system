namespace LeaveManagement.API.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty ;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public String Status { get; set; } = "Unread";

        //Navigation property
        public virtual Employee? Employee { get; set; } 
    }
}
