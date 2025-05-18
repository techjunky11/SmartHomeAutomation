using Microsoft.EntityFrameworkCore;
using SmartHomeAutomation.Api.Data;
using SmartHomeAutomation.Api.Models;

namespace SmartHomeAutomation.Api.Repositories
{
    public class SensorRepository : ISensorRepository
    {
        private readonly ApplicationDbContext _context;

        public SensorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SensorData>> GetHistoricalSensorDataAsync(string type, DateTime startDate, DateTime endDate)
        {
            // Get historical data and resample if needed to prevent returning too many data points
            var query = _context.SensorData
                .Where(s => s.Timestamp >= startDate && s.Timestamp <= endDate)
                .OrderBy(s => s.Timestamp);

            // If the time range is large, resample to reduce data points
            TimeSpan timeSpan = endDate - startDate;

            if (timeSpan.TotalDays > 7)
            {
                // For longer time periods, resample to hourly data
                return await query
                    .GroupBy(s => new { s.Timestamp.Hour, s.Timestamp.Day, s.Timestamp.Month, s.Timestamp.Year })
                    .Select(g => new SensorData
                    {
                        Temperature = g.Average(s => s.Temperature),
                        Humidity = g.Average(s => s.Humidity),
                        LightLevel = (int)g.Average(s => s.LightLevel),
                        MotionDetected = g.Any(s => s.MotionDetected),
                        Distance = (int)g.Average(s => s.Distance),
                        Timestamp = g.Min(s => s.Timestamp)
                    })
                    .OrderBy(s => s.Timestamp)
                    .ToListAsync();
            }
            else if (timeSpan.TotalDays > 1)
            {
                // For days, resample to data every 15 minutes
                return await query
                    .GroupBy(s => new
                    {
                        FifteenMin = s.Timestamp.Minute / 15,
                        s.Timestamp.Hour,
                        s.Timestamp.Day,
                        s.Timestamp.Month,
                        s.Timestamp.Year
                    })
                    .Select(g => new SensorData
                    {
                        Temperature = g.Average(s => s.Temperature),
                        Humidity = g.Average(s => s.Humidity),
                        LightLevel = (int)g.Average(s => s.LightLevel),
                        MotionDetected = g.Any(s => s.MotionDetected),
                        Distance = (int)g.Average(s => s.Distance),
                        Timestamp = g.Min(s => s.Timestamp)
                    })
                    .OrderBy(s => s.Timestamp)
                    .ToListAsync();
            }
            else
            {
                // For shorter time periods, get all data points
                return await query.ToListAsync();
            }
        }

        public async Task SaveSensorDataAsync(SensorData data)
        {
            _context.SensorData.Add(data);
            await _context.SaveChangesAsync();
        }

        public async Task<DeviceStatus> GetLatestDeviceStatusAsync()
        {
            return await _context.DeviceStatus
                .OrderByDescending(d => d.Timestamp)
                .FirstOrDefaultAsync() ?? new DeviceStatus();
        }

        public async Task SaveDeviceStatusAsync(DeviceStatus status)
        {
            _context.DeviceStatus.Add(status);
            await _context.SaveChangesAsync();
        }
    }
}