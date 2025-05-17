using SmartHomeAutomation.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeAutomation.Api.Repositories
{
    public interface IAlertRepository
    {
        Task<List<AlertData>> GetHistoricalAlertsAsync(DateTime startDate, DateTime endDate);
        Task SaveAlertAsync(AlertData alert);
    }
}
