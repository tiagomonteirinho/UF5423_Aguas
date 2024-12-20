using Microsoft.EntityFrameworkCore.Migrations;

namespace Water_Services.Migrations
{
    public partial class AddMeterSerialNumbers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Meters");

            migrationBuilder.AddColumn<int>(
                name: "SerialNumber",
                table: "Meters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "Meters");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Meters",
                type: "nvarchar(99)",
                maxLength: 99,
                nullable: false,
                defaultValue: "");
        }
    }
}
