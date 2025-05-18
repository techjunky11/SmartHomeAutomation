using Microsoft.EntityFrameworkCore;
using SmartHomeAutomation.Api.Data;
using SmartHomeAutomation.Api.Repositories;
using SmartHomeAutomation.Api.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy => policy
            .WithOrigins("http://localhost:3000", "http://192.168.1.69:3000") // Update with your React app URL
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Add SignalR
builder.Services.AddSignalR();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<ISensorRepository, SensorRepository>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();

// Register Arduino service as a singleton

builder.Services.AddSingleton<ArduinoService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<ArduinoService>());

//builder.Services.AddHostedService<ArduinoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");
app.UseAuthorization();

app.MapControllers();
app.MapHub<SmartHomeHub>("/smartHomeHub");

app.Run();