using Microsoft.AspNetCore.SignalR;
using SmartHomeAutomation.Api.Models;
using SmartHomeAutomation.Api.Repositories;
using System.IO.Ports;
using System.Text.Json;

namespace SmartHomeAutomation.Api.Service
{
    public class ArduinoService : IHostedService, IDisposable
    {
        private readonly ILogger<ArduinoService> _logger;
        private readonly IHubContext<SmartHomeHub> _hubContext;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _portName;
        private readonly int _baudRate;
        private SerialPort _serialPort;
        private bool _isConnected = false;
        private Timer _reconnectTimer;
        private readonly object _lock = new object();

        public ArduinoService(
            ILogger<ArduinoService> logger,
            IHubContext<SmartHomeHub> hubContext,
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _hubContext = hubContext;
            _scopeFactory = scopeFactory;
            _portName = configuration["Arduino:PortName"] ?? "COM16";
            _baudRate = int.Parse(configuration["Arduino:BaudRate"] ?? "115200");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Arduino service starting");
            InitializeSerialPort();

            _reconnectTimer = new Timer(CheckConnection, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }

        private void InitializeSerialPort()
        {
            try
            {
                lock (_lock)
                {
                    if (_serialPort != null && _serialPort.IsOpen)
                    {
                        _serialPort.Close();
                        _serialPort.Dispose();
                    }

                    _serialPort = new SerialPort
                    {
                        PortName = _portName,
                        BaudRate = _baudRate,
                        DataBits = 8,
                        Parity = Parity.None,
                        StopBits = StopBits.One,
                        ReadTimeout = 3000,
                        WriteTimeout = 3000
                    };

                    _serialPort.DataReceived += SerialPortDataReceived;

                    _serialPort.Open();
                    _isConnected = true;
                    _logger.LogInformation("Connected to Arduino on port {PortName}", _portName);
                }
            }
            catch (Exception ex)
            {
                _isConnected = false;
                _logger.LogError(ex, "Error connecting to Arduino on port {PortName}", _portName);
            }
        }

        private void CheckConnection(object state)
        {
            if (!_isConnected)
            {
                _logger.LogWarning("Arduino connection is down, attempting to reconnect");
                InitializeSerialPort();
            }
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (_serialPort == null || !_serialPort.IsOpen) return;

            try
            {
                string data = _serialPort.ReadLine();
                _logger.LogDebug("Received data: {Data}", data);

                ProcessArduinoData(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Arduino data");
            }
        }

        private async void ProcessArduinoData(string jsonData)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(jsonData))
                {
                    JsonElement root = doc.RootElement;
                    string type = root.GetProperty("type").GetString();

                    switch (type)
                    {
                        case "sensor_data":
                            await ProcessSensorData(root);
                            break;

                        case "device_status": // Add this case
                            await ProcessDeviceStatus(root);
                            break;

                        case "system_status":
                            await ProcessSystemStatus(root);
                            break;

                        case "alert":
                            await ProcessAlert(root);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing JSON data: {JsonData}", jsonData);
            }
        }

        // Add this method to handle device status messages
        private async Task ProcessDeviceStatus(JsonElement data)
        {
            try
            {
                var deviceStatus = new DeviceStatus
                {
                    DoorStatus = data.GetProperty("DoorStatus").GetBoolean(),
                    FanStatus = data.GetProperty("FanStatus").GetBoolean(),
                    LightStatus = data.GetProperty("LightStatus").GetBoolean(),
                    Timestamp = DateTime.Now
                };

                using var scope = _scopeFactory.CreateScope();
                var sensorRepo = scope.ServiceProvider.GetRequiredService<ISensorRepository>();
                await sensorRepo.SaveDeviceStatusAsync(deviceStatus);

                await _hubContext.Clients.All.SendAsync("ReceiveDeviceStatus", deviceStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing device status data");
            }
        }

        private async Task ProcessSensorData(JsonElement data)
        {
            var sensorData = new SensorData
            {
                Temperature = data.GetProperty("temperature").GetSingle(),
                Humidity = data.GetProperty("humidity").GetSingle(),
                Distance = data.GetProperty("distance").GetInt32(),
                LightLevel = data.GetProperty("lightLevel").GetInt32(),
                MotionDetected = data.GetProperty("motionDetected").GetBoolean()
            };

            using var scope = _scopeFactory.CreateScope();
            var sensorRepo = scope.ServiceProvider.GetRequiredService<ISensorRepository>();
            await sensorRepo.SaveSensorDataAsync(sensorData);

            await _hubContext.Clients.All.SendAsync("ReceiveSensorData", sensorData);
        }

        private async Task ProcessSystemStatus(JsonElement data)
        {
            var sensorData = new SensorData
            {
                Temperature = data.GetProperty("temperature").GetSingle(),
                Humidity = data.GetProperty("humidity").GetSingle(),
                Distance = data.GetProperty("distance").GetInt32(),
                LightLevel = data.GetProperty("lightLevel").GetInt32(),
                MotionDetected = data.GetProperty("motionDetected").GetBoolean(),
            };

            var deviceStatus = new DeviceStatus
            {
                // Updated to use property names matching what Arduino sends
                // Match exactly what's in DeviceStatus.cs
                DoorStatus = data.GetProperty("DoorStatus").GetBoolean(),
                FanStatus = data.GetProperty("FanStatus").GetBoolean(),
                LightStatus = data.GetProperty("LightStatus").GetBoolean(),
                Timestamp = DateTime.Now
            };

            using var scope = _scopeFactory.CreateScope();
            var sensorRepo = scope.ServiceProvider.GetRequiredService<ISensorRepository>();
            await sensorRepo.SaveSensorDataAsync(sensorData);
            await sensorRepo.SaveDeviceStatusAsync(deviceStatus);

            await _hubContext.Clients.All.SendAsync("ReceiveSensorData", sensorData);
            await _hubContext.Clients.All.SendAsync("ReceiveDeviceStatus", deviceStatus);
        }

        private async Task ProcessAlert(JsonElement data)
        {
            var alertData = new AlertData
            {
                // Keep this field if your AlertData model has a Type property
                Type = data.TryGetProperty("type", out var typeElement) ? typeElement.GetString() : "alert",

                AlertType = data.GetProperty("alert_type").GetString(),
                Message = data.GetProperty("message").GetString(),
                Source = data.TryGetProperty("source", out var sourceElement) ? sourceElement.GetString() : "unknown",
                Timestamp = DateTime.Now
            };

            using var scope = _scopeFactory.CreateScope();
            var alertRepo = scope.ServiceProvider.GetRequiredService<IAlertRepository>();
            await alertRepo.SaveAlertAsync(alertData);

            await _hubContext.Clients.All.SendAsync("ReceiveAlertData", alertData);
        }

        public async Task SendCommandToArduino(string deviceType, bool action)
        {
            try
            {
                lock (_lock)
                {
                    if (_serialPort == null || !_serialPort.IsOpen)
                    {
                        throw new Exception("Serial port is not open");
                    }

                    string command = deviceType.ToLower() switch
                    {
                        "door" => JsonSerializer.Serialize(new { command = "door", IsDoorOpen = action }),
                        "fan" => JsonSerializer.Serialize(new { command = "fan", IsFanOn = action }),
                        "light" => JsonSerializer.Serialize(new { command = "lights", IsLightOn = action }),
                        _ => throw new ArgumentException($"Unknown device type: {deviceType}")
                    };

                    _serialPort.WriteLine(command);
                    _logger.LogInformation("Sent command to Arduino: {Command}", command);
                }

                using var scope = _scopeFactory.CreateScope();
                var sensorRepo = scope.ServiceProvider.GetRequiredService<ISensorRepository>();
                var currentStatus = await sensorRepo.GetLatestDeviceStatusAsync();

                var newStatus = new DeviceStatus
                {
                    DoorStatus = deviceType == "door" ? action : currentStatus.DoorStatus,
                    FanStatus = deviceType == "fan" ? action : currentStatus.FanStatus,
                    LightStatus = deviceType == "light" ? action : currentStatus.LightStatus,
                    Timestamp = DateTime.Now
                };

                await sensorRepo.SaveDeviceStatusAsync(newStatus);
                await _hubContext.Clients.All.SendAsync("ReceiveDeviceStatus", newStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending command to Arduino");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Arduino service stopping");

            lock (_lock)
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.Close();
                    _isConnected = false;
                }
            }

            _reconnectTimer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (_serialPort != null)
                {
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Close();
                    }
                    _serialPort.Dispose();
                    _serialPort = null;
                }
            }

            _reconnectTimer?.Dispose();
        }
    }
}