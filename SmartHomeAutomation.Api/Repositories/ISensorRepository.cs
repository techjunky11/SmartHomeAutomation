using SmartHomeAutomation.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeAutomation.Api.Repositories
{
    public interface ISensorRepository
    {
        Task<List<SensorData>> GetHistoricalSensorDataAsync(string type, DateTime startDate, DateTime endDate);
        Task SaveSensorDataAsync(SensorData data);
        Task<DeviceStatus> GetLatestDeviceStatusAsync();
        Task SaveDeviceStatusAsync(DeviceStatus status);
    }
}
