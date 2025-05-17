using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeAutomation.Api.Models
{
    public class SensorData
    {
        public int Id { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public int Distance { get; set; }
        public int LightLevel { get; set; }
        public bool MotionDetected { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

}
