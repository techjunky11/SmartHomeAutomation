using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHomeAutomation.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoorStatus",
                table: "SensorData");

            migrationBuilder.DropColumn(
                name: "FanStatus",
                table: "SensorData");

            migrationBuilder.DropColumn(
                name: "LightStatus",
                table: "SensorData");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DoorStatus",
                table: "SensorData",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FanStatus",
                table: "SensorData",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LightStatus",
                table: "SensorData",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
