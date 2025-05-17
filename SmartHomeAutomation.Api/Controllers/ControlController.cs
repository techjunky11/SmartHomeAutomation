using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHomeAutomation.Api.Models;
using SmartHomeAutomation.Api.Service;

namespace SmartHomeAutomation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControlController : ControllerBase
    {
        private readonly ILogger<ControlController> _logger;
        private readonly ArduinoService _arduinoService;

        public ControlController(
            ILogger<ControlController> logger,
            ArduinoService arduinoService)
        {
            _logger = logger;
            _arduinoService = arduinoService;
        }

        [HttpPost("{deviceType}")]
        public async Task<ActionResult> ControlDevice(string deviceType, [FromBody] ControlCommand command)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceType))
                {
                    return BadRequest("Device type is required");
                }

                deviceType = deviceType.ToLower();
                if (deviceType != "door" && deviceType != "fan" && deviceType != "light")
                {
                    return BadRequest($"Invalid device type: {deviceType}");
                }

                await _arduinoService.SendCommandToArduino(deviceType, command.Action);

                return Ok(new { success = true, message = $"{deviceType} state changed to {command.Action}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error controlling device {DeviceType}", deviceType);
                return StatusCode(500, new { success = false, message = $"Error controlling {deviceType}: {ex.Message}" });
            }
        }
    }
}
