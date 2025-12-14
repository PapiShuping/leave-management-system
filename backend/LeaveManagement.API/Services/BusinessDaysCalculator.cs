using LeaveManagement.API.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.API.Services
{
    public interface IBusinessDaysCalculator
    {
        Task<int> CalculateBusinessDays(DateTime startDate, DateTime endDate);
    }

    public class BusinessDaysCalculator : IBusinessDaysCalculator
    {
        private readonly ApplicationDbContext _context;

        public BusinessDaysCalculator(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CalculateBusinessDays(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("Start date must be before or equal to end date");
            }

            // Get all holidays between the dates
            var holidays = await _context.Holidays
                .Where(h => h.Date >= startDate && h.Date <= endDate)
                .Select(h => h.Date.Date)
                .ToListAsync();

            int businessDays = 0;

            // Count each day
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                // Skip if it's a holiday (weekend or public holiday)
                if (!holidays.Contains(date))
                {
                    businessDays++;
                }
            }

            return businessDays;
        }
    }
}