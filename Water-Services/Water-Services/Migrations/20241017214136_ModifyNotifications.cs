using Microsoft.EntityFrameworkCore.Migrations;

namespace Water_Services.Migrations
{
    public partial class ModifyNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SenderName",
                table: "Notifications",
                newName: "NewAccountEmail");

            migrationBuilder.RenameColumn(
                name: "SenderEmail",
                table: "Notifications",
                newName: "Action");

            migrationBuilder.AddColumn<bool>(
                name: "Read",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "Volume",
                table: "Consumptions",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Read",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "NewAccountEmail",
                table: "Notifications",
                newName: "SenderName");

            migrationBuilder.RenameColumn(
                name: "Action",
                table: "Notifications",
                newName: "SenderEmail");

            migrationBuilder.AlterColumn<decimal>(
                name: "Volume",
                table: "Consumptions",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
