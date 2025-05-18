using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeAutomation.Api.Models
{
    public class AlertData
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string AlertType { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public DateTime Timestamp { get; set; }
    }
}