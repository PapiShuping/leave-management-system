using LeaveManagement.API.Models;

namespace LeaveManagement.API.Data
{
    public static class DbSeeder
    {
        public static void SeedData(ApplicationDbContext context)
        {
            // Seed Holidays
            SeedHolidays(context);

            // Seed Sample Employees
            SeedEmployees(context);

            // Seed Leave Balances
            SeedLeaveBalances(context);
        }

        private static void SeedHolidays(ApplicationDbContext context)
        {
            // Check if holidays already exist
            if (context.Holidays.Any())
            {
                return; // Database already seeded
            }

            var holidays = new List<Holiday>();

            // South African Public Holidays 2025
            var publicHolidays = new Dictionary<string, DateTime>
            {
                { "New Year's Day", new DateTime(2025, 1, 1) },
                { "Human Rights Day", new DateTime(2025, 3, 21) },
                { "Good Friday", new DateTime(2025, 4, 18) },
                { "Family Day", new DateTime(2025, 4, 21) },
                { "Freedom Day", new DateTime(2025, 4, 27) },
                { "Workers' Day", new DateTime(2025, 5, 1) },
                { "Youth Day", new DateTime(2025, 6, 16) },
                { "National Women's Day", new DateTime(2025, 8, 9) },
                { "Heritage Day", new DateTime(2025, 9, 24) },
                { "Day of Reconciliation", new DateTime(2025, 12, 16) },
                { "Christmas Day", new DateTime(2025, 12, 25) },
                { "Day of Goodwill", new DateTime(2025, 12, 26) }
            };

            // Add public holidays
            foreach (var holiday in publicHolidays)
            {
                holidays.Add(new Holiday
                {
                    Name = holiday.Key,
                    Date = holiday.Value,
                    IsWeekend = false
                });
            }

            // Add all weekends for 2025
            var startDate = new DateTime(2025, 1, 1);
            var endDate = new DateTime(2025, 12, 31);

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    holidays.Add(new Holiday
                    {
                        Name = "Weekend",
                        Date = date,
                        IsWeekend = true
                    });
                }
            }

            context.Holidays.AddRange(holidays);
            context.SaveChanges();
        }

        private static void SeedEmployees(ApplicationDbContext context)
        {
            // Check if employees already exist
            if (context.Employees.Any())
            {
                return;
            }

            var employees = new List<Employee>
            {
                new Employee
                {
                    EmployeeId = "E001",
                    Name = "John Doe",
                    Email = "john.doe@company.com",
                    Department = "Finance",
                    Role = "Employee",
                    ManagerId = "001"
                },
                new Employee
                {
                    EmployeeId = "E002",
                    Name = "Jane Smith",
                    Email = "jane.smith@company.com",
                    Department = "IT",
                    Role = "Employee",
                    ManagerId = "002"
                },
                new Employee
                {
                    EmployeeId = "E003",
                    Name = "Mike Johnson",
                    Email = "mike.johnson@company.com",
                    Department = "HR",
                    Role = "Employee",
                    ManagerId = "003"
                },
                new Employee
                {
                    EmployeeId = "M001",
                    Name = "Sarah Williams",
                    Email = "sarah.williams@company.com",
                    Department = "Finance",
                    Role = "Manager",
                    ManagerId = "M001"
                },
                new Employee
                {
                    EmployeeId = "M002",
                    Name = "David Brown",
                    Email = "david.brown@company.com",
                    Department = "IT",
                    Role = "Manager",
                    ManagerId = "M002"
                },
                new Employee
                {
                    EmployeeId = "M003",
                    Name = "Lisa Davis",
                    Email = "lisa.davis@company.com",
                    Department = "HR",
                    Role = "HR",
                    ManagerId = "M003"
                }
            };

            context.Employees.AddRange(employees);
            context.SaveChanges();
        }

        private static void SeedLeaveBalances(ApplicationDbContext context)
        {
            // Check if leave balances already exist
            if (context.LeaveBalances.Any())
            {
                return;
            }

            var employees = context.Employees.ToList();
            var leaveBalances = new List<LeaveBalance>();

            // Standard leave allocations
            var leaveTypes = new Dictionary<string, int>
            {
                { "Annual Leave", 21 },
                { "Sick Leave", 30 },
                { "Family Responsibility Leave", 3 },
                { "Maternity Leave", 130 }
            };

            // Create leave balances for each employee
            foreach (var employee in employees)
            {
                foreach (var leaveType in leaveTypes)
                {
                    leaveBalances.Add(new LeaveBalance
                    {
                        EmployeeId = employee.EmployeeId,
                        EmployeeName = employee.Name,
                        LeaveType = leaveType.Key,
                        DaysAllocated = leaveType.Value,
                        DaysUsed = 0,
                        DaysLeft = leaveType.Value,
                        LastUpdated = DateTime.Now
                    });
                }
            }

            context.LeaveBalances.AddRange(leaveBalances);
            context.SaveChanges();
        }
    }
}