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
builder.Services.AddCors(opt=>
{
    opt.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()// Replace with your Blazor WASM URL
        .AllowAnyMethod()
        .AllowAnyHeader());

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
builder.Services.AddHostedService<ArduinoService>();



var app = builder.Build();

// Configure the HTTP request pipeline.

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");
app.UseAuthorization();

app.MapControllers();
app.MapHub<SmartHomeHub>("/smartHomeHub");

app.Run();
