using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeaveManagement.API.Data;
using LeaveManagement.API.DTOs;
using LeaveManagement.API.Models;
using LeaveManagement.API.Services;

namespace LeaveManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBusinessDaysCalculator _businessDaysCalculator;

        public LeaveRequestsController(
            ApplicationDbContext context,
            IBusinessDaysCalculator businessDaysCalculator)
        {
            _context = context;
            _businessDaysCalculator = businessDaysCalculator;
        }

        // GET: api/LeaveRequests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetLeaveRequests()
        {
            var requests = await _context.LeaveRequests
                .OrderByDescending(lr => lr.CreatedAt)
                .Select(lr => new LeaveRequestDto
                {
                    Id = lr.Id,
                    EmployeeId = lr.EmployeeId,
                    EmployeeName = lr.EmployeeName,
                    EmployeeEmail = lr.EmployeeEmail,
                    LeaveType = lr.LeaveType,
                    StartDate = lr.StartDate,
                    EndDate = lr.EndDate,
                    Reason = lr.Reason,
                    FilePath = lr.FilePath,
                    Status = lr.Status,
                    ApprovedBy = lr.ApprovedBy,
                    RejectComment = lr.RejectComment,
                    CreatedAt = lr.CreatedAt,
                    DaysTaken = lr.DaysTaken
                })
                .ToListAsync();

            return Ok(requests);
        }

        // GET: api/LeaveRequests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequestDto>> GetLeaveRequest(int id)
        {
            var request = await _context.LeaveRequests
                .Where(lr => lr.Id == id)
                .Select(lr => new LeaveRequestDto
                {
                    Id = lr.Id,
                    EmployeeId = lr.EmployeeId,
                    EmployeeName = lr.EmployeeName,
                    EmployeeEmail = lr.EmployeeEmail,
                    LeaveType = lr.LeaveType,
                    StartDate = lr.StartDate,
                    EndDate = lr.EndDate,
                    Reason = lr.Reason,
                    FilePath = lr.FilePath,
                    Status = lr.Status,
                    ApprovedBy = lr.ApprovedBy,
                    RejectComment = lr.RejectComment,
                    CreatedAt = lr.CreatedAt,
                    DaysTaken = lr.DaysTaken
                })
                .FirstOrDefaultAsync();

            if (request == null)
            {
                return NotFound(new { message = $"Leave request with ID {id} not found" });
            }

            return Ok(request);
        }

        // GET: api/LeaveRequests/employee/E001
        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetEmployeeLeaveRequests(string employeeId)
        {
            var requests = await _context.LeaveRequests
                .Where(lr => lr.EmployeeId == employeeId)
                .OrderByDescending(lr => lr.CreatedAt)
                .Select(lr => new LeaveRequestDto
                {
                    Id = lr.Id,
                    EmployeeId = lr.EmployeeId,
                    EmployeeName = lr.EmployeeName,
                    EmployeeEmail = lr.EmployeeEmail,
                    LeaveType = lr.LeaveType,
                    StartDate = lr.StartDate,
                    EndDate = lr.EndDate,
                    Reason = lr.Reason,
                    FilePath = lr.FilePath,
                    Status = lr.Status,
                    ApprovedBy = lr.ApprovedBy,
                    RejectComment = lr.RejectComment,
                    CreatedAt = lr.CreatedAt,
                    DaysTaken = lr.DaysTaken
                })
                .ToListAsync();

            return Ok(requests);
        }

        // GET: api/LeaveRequests/status/Pending
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetLeaveRequestsByStatus(string status)
        {
            var requests = await _context.LeaveRequests
                .Where(lr => lr.Status.ToLower() == status.ToLower())
                .OrderByDescending(lr => lr.CreatedAt)
                .Select(lr => new LeaveRequestDto
                {
                    Id = lr.Id,
                    EmployeeId = lr.EmployeeId,
                    EmployeeName = lr.EmployeeName,
                    EmployeeEmail = lr.EmployeeEmail,
                    LeaveType = lr.LeaveType,
                    StartDate = lr.StartDate,
                    EndDate = lr.EndDate,
                    Reason = lr.Reason,
                    FilePath = lr.FilePath,
                    Status = lr.Status,
                    ApprovedBy = lr.ApprovedBy,
                    RejectComment = lr.RejectComment,
                    CreatedAt = lr.CreatedAt,
                    DaysTaken = lr.DaysTaken
                })
                .ToListAsync();

            return Ok(requests);
        }

        // POST: api/LeaveRequests
        [HttpPost]
        public async Task<ActionResult<LeaveRequestDto>> CreateLeaveRequest(CreateLeaveRequestDto dto)
        {
            // Validate employee exists
            var employee = await _context.Employees.FindAsync(dto.EmployeeId);
            if (employee == null)
            {
                return BadRequest(new { message = "Employee not found" });
            }

            // Validate dates
            if (dto.StartDate > dto.EndDate)
            {
                return BadRequest(new { message = "Start date must be before or equal to end date" });
            }

            if (dto.StartDate < DateTime.Today)
            {
                return BadRequest(new { message = "Cannot request leave for past dates" });
            }

            // Calculate business days
            int daysTaken = await _businessDaysCalculator.CalculateBusinessDays(dto.StartDate, dto.EndDate);

            if (daysTaken == 0)
            {
                return BadRequest(new { message = "Selected dates contain only holidays/weekends" });
            }

            // Check if employee has enough leave balance
            var leaveBalance = await _context.LeaveBalances
                .FirstOrDefaultAsync(lb => lb.EmployeeId == dto.EmployeeId && lb.LeaveType == dto.LeaveType);

            if (leaveBalance == null)
            {
                return BadRequest(new { message = $"Leave type '{dto.LeaveType}' not found for employee" });
            }

            if (leaveBalance.DaysLeft < daysTaken)
            {
                return BadRequest(new 
                { 
                    message = $"Insufficient leave balance. You have {leaveBalance.DaysLeft} days left, but requesting {daysTaken} days" 
                });
            }

            // Create leave request
            var leaveRequest = new LeaveRequest
            {
                EmployeeId = dto.EmployeeId,
                EmployeeName = employee.Name,
                EmployeeEmail = employee.Email,
                LeaveType = dto.LeaveType,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Reason = dto.Reason,
                FilePath = dto.FilePath,
                Status = "Pending",
                CreatedAt = DateTime.Now,
                DaysTaken = daysTaken
            };

            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

            var result = new LeaveRequestDto
            {
                Id = leaveRequest.Id,
                EmployeeId = leaveRequest.EmployeeId,
                EmployeeName = leaveRequest.EmployeeName,
                EmployeeEmail = leaveRequest.EmployeeEmail,
                LeaveType = leaveRequest.LeaveType,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                Reason = leaveRequest.Reason,
                FilePath = leaveRequest.FilePath,
                Status = leaveRequest.Status,
                CreatedAt = leaveRequest.CreatedAt,
                DaysTaken = leaveRequest.DaysTaken
            };

            return CreatedAtAction(nameof(GetLeaveRequest), new { id = result.Id }, result);
        }

        // PUT: api/LeaveRequests/5/approve
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveLeaveRequest(int id, ApproveLeaveRequestDto dto)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            
            if (leaveRequest == null)
            {
                return NotFound(new { message = "Leave request not found" });
            }

            if (leaveRequest.Status != "Pending")
            {
                return BadRequest(new { message = $"Cannot approve request with status: {leaveRequest.Status}" });
            }

            // Update leave balance
            var leaveBalance = await _context.LeaveBalances
                .FirstOrDefaultAsync(lb => lb.EmployeeId == leaveRequest.EmployeeId && lb.LeaveType == leaveRequest.LeaveType);

            if (leaveBalance == null)
            {
                return BadRequest(new { message = "Leave balance not found" });
            }

            if (leaveBalance.DaysLeft < leaveRequest.DaysTaken)
            {
                return BadRequest(new { message = "Insufficient leave balance" });
            }

            // Deduct from balance
            leaveBalance.DaysUsed += leaveRequest.DaysTaken ?? 0;
            leaveBalance.DaysLeft = leaveBalance.DaysAllocated - leaveBalance.DaysUsed;
            leaveBalance.LastUpdated = DateTime.Now;

            // Update request status
            leaveRequest.Status = "Approved";
            leaveRequest.ApprovedBy = dto.ApprovedBy;

            // Create notification
            var notification = new Notification
            {
                EmployeeId = leaveRequest.EmployeeId,
                Title = "Leave Request Approved",
                Message = $"Your {leaveRequest.LeaveType} request from {leaveRequest.StartDate:yyyy-MM-dd} to {leaveRequest.EndDate:yyyy-MM-dd} has been approved.",
                CreatedAt = DateTime.Now,
                Status = "Unread"
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Leave request approved successfully" });
        }

        // PUT: api/LeaveRequests/5/reject
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectLeaveRequest(int id, RejectLeaveRequestDto dto)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            
            if (leaveRequest == null)
            {
                return NotFound(new { message = "Leave request not found" });
            }

            if (leaveRequest.Status != "Pending")
            {
                return BadRequest(new { message = $"Cannot reject request with status: {leaveRequest.Status}" });
            }

            // Update request status
            leaveRequest.Status = "Rejected";
            leaveRequest.RejectComment = dto.RejectComment;
            leaveRequest.ApprovedBy = dto.RejectedBy;

            // Create notification
            var notification = new Notification
            {
                EmployeeId = leaveRequest.EmployeeId,
                Title = "Leave Request Rejected",
                Message = $"Your {leaveRequest.LeaveType} request from {leaveRequest.StartDate:yyyy-MM-dd} to {leaveRequest.EndDate:yyyy-MM-dd} has been rejected. Reason: {dto.RejectComment}",
                CreatedAt = DateTime.Now,
                Status = "Unread"
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Leave request rejected successfully" });
        }
    }
}