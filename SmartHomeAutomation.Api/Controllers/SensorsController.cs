using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHomeAutomation.Api.Models;
using SmartHomeAutomation.Api.Repositories;

namespace SmartHomeAutomation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly ILogger<SensorsController> _logger;
        private readonly ISensorRepository _sensorRepository;

        public SensorsController(
            ILogger<SensorsController> logger,
            ISensorRepository sensorRepository)
        {
            _logger = logger;
            _sensorRepository = sensorRepository;
        }

        [HttpGet("historical")]
        public async Task<ActionResult<IEnumerable<SensorData>>> GetHistoricalSensorData(
            [FromQuery] string type,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var data = await _sensorRepository.GetHistoricalSensorDataAsync(type, startDate, endDate);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving historical sensor data");
                return StatusCode(500, "An error occurred while retrieving historical sensor data");
            }
        }

        [HttpGet("status")]
        public async Task<ActionResult<DeviceStatus>> GetCurrentDeviceStatus()
        {
            try
            {
                var status = await _sensorRepository.GetLatestDeviceStatusAsync();
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current device status");
                return StatusCode(500, "An error occurred while retrieving current device status");
            }
        }
    }
}
