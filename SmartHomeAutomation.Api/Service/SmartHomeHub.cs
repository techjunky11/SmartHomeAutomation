using Microsoft.AspNetCore.SignalR;
using SmartHomeAutomation.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeAutomation.Api.Service
{
    public class SmartHomeHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            await Clients.Caller.SendAsync("ReceiveConnectionId", connectionId);
            await base.OnConnectedAsync();
        }

        public async Task SendSensorData(SensorData data)
        {
            await Clients.All.SendAsync("ReceiveSensorData", data);
        }

        public async Task SendAlertData(AlertData data)
        {
            await Clients.All.SendAsync("ReceiveAlertData", data);
        }

        public async Task SendDeviceStatus(DeviceStatus data)
        {
            await Clients.All.SendAsync("ReceiveDeviceStatus", data);
        }
    }

}
