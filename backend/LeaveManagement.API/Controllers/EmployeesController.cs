using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeaveManagement.API.Data;
using LeaveManagement.API.DTOs;

namespace LeaveManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "Controller is working!", timestamp = DateTime.Now });
        }
        // TEST database connection
        [HttpGet("test-db")]
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                var count = await _context.Employees.CountAsync();
                var firstEmployee = await _context.Employees.FirstOrDefaultAsync();

                return Ok(new
                {
                    message = "Database connected!",
                    employeeCount = count,
                    firstEmployeeName = firstEmployee?.Name ?? "No employees found"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var employees = await _context.Employees
                .Select(e => new EmployeeDto
                {
                    EmployeeId = e.EmployeeId,
                    Name = e.Name,
                    Email = e.Email,
                    Department = e.Department,
                    Role = e.Role,
                    ManagerId = e.ManagerId
                })
                .ToListAsync();

            return Ok(employees);
        }

        // GET: api/Employees/E001
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(string id)
        {
            var employee = await _context.Employees
                .Where(e => e.EmployeeId == id)
                .Select(e => new EmployeeDto
                {
                    EmployeeId = e.EmployeeId,
                    Name = e.Name,
                    Email = e.Email,
                    Department = e.Department,
                    Role = e.Role,
                    ManagerId = e.ManagerId
                })
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                return NotFound(new { message = $"Employee with ID {id} not found" });
            }

            return Ok(employee);
        }

        // GET: api/Employees/E001/balances
        [HttpGet("{id}/balances")]
        public async Task<ActionResult<EmployeeWithBalancesDto>> GetEmployeeWithBalances(string id)
        {
            var employee = await _context.Employees
                .Where(e => e.EmployeeId == id)
                .Include(e => e.LeaveBalances)
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                return NotFound(new { message = $"Employee with ID {id} not found" });
            }

            var employeeDto = new EmployeeWithBalancesDto
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                Role = employee.Role,
                LeaveBalances = employee.LeaveBalances.Select(lb => new LeaveBalanceDto
                {
                    LeaveType = lb.LeaveType,
                    DaysAllocated = lb.DaysAllocated,
                    DaysUsed = lb.DaysUsed,
                    DaysLeft = lb.DaysLeft
                }).ToList()
            };

            return Ok(employeeDto);
        }

        // GET: api/Employees/department/Finance
        [HttpGet("department/{department}")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesByDepartment(string department)
        {
            var employees = await _context.Employees
                .Where(e => e.Department.ToLower() == department.ToLower())
                .Select(e => new EmployeeDto
                {
                    EmployeeId = e.EmployeeId,
                    Name = e.Name,
                    Email = e.Email,
                    Department = e.Department,
                    Role = e.Role,
                    ManagerId = e.ManagerId
                })
                .ToListAsync();

            if (!employees.Any())
            {
                return NotFound(new { message = $"No employees found in {department} department" });
            }

            return Ok(employees);
        }

        // GET: api/Employees/managers
        [HttpGet("managers")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetManagers()
        {
            var managers = await _context.Employees
                .Where(e => e.Role == "Manager" || e.Role == "HR")
                .Select(e => new EmployeeDto
                {
                    EmployeeId = e.EmployeeId,
                    Name = e.Name,
                    Email = e.Email,
                    Department = e.Department,
                    Role = e.Role,
                    ManagerId = e.ManagerId
                })
                .ToListAsync();

            return Ok(managers);
        }
    }
}