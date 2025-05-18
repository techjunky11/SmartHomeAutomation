using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHomeAutomation.Api.Models;
using SmartHomeAutomation.Api.Repositories;

namespace SmartHomeAutomation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertsController : ControllerBase
    {
        private readonly ILogger<AlertsController> _logger;
        private readonly IAlertRepository _alertRepository;

        public AlertsController(
            ILogger<AlertsController> logger,
            IAlertRepository alertRepository)
        {
            _logger = logger;
            _alertRepository = alertRepository;
        }

        [HttpGet("historical")]
        public async Task<ActionResult<IEnumerable<AlertData>>> GetHistoricalAlerts(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var alerts = await _alertRepository.GetHistoricalAlertsAsync(startDate, endDate);
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving historical alerts");
                return StatusCode(500, "An error occurred while retrieving historical alerts");
            }
        }
    }
}