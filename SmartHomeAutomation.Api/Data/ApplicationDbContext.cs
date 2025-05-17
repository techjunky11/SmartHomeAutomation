using Microsoft.EntityFrameworkCore;
using SmartHomeAutomation.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeAutomation.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SensorData> SensorData { get; set; }
        public DbSet<DeviceStatus> DeviceStatus { get; set; }
        public DbSet<AlertData> AlertData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure indexes
            modelBuilder.Entity<SensorData>()
                .HasIndex(s => s.Timestamp);

            modelBuilder.Entity<AlertData>()
                .HasIndex(a => a.Timestamp);

            modelBuilder.Entity<DeviceStatus>()
                .HasIndex(d => d.Timestamp);
        }
    }
}

