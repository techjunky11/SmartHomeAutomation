using Microsoft.EntityFrameworkCore;
using SmartHomeAutomation.Api.Data;
using SmartHomeAutomation.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeAutomation.Api.Repositories
{
    public class AlertRepository : IAlertRepository
    {
        private readonly ApplicationDbContext _context;

        public AlertRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AlertData>> GetHistoricalAlertsAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.AlertData
                .Where(a => a.Timestamp >= startDate && a.Timestamp <= endDate)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task SaveAlertAsync(AlertData alert)
        {
            _context.AlertData.Add(alert);
            await _context.SaveChangesAsync();
        }
    }
}
