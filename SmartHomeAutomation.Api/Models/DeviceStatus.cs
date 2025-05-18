using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeAutomation.Api.Models
{
    public class DeviceStatus
    {
        public int Id { get; set; }
        public bool DoorStatus { get; set; }
        public bool FanStatus { get; set; }
        public bool LightStatus { get; set; }
        public DateTime Timestamp { get; set; }
    }
}