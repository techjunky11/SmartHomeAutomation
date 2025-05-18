using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHomeAutomation.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Distance",
                table: "SensorData",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

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

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "AlertData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "AlertData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "Source",
                table: "AlertData");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AlertData");

            migrationBuilder.AlterColumn<int>(
                name: "Distance",
                table: "SensorData",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
