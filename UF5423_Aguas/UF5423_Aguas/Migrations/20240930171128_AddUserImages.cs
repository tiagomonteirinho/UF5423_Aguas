using Microsoft.EntityFrameworkCore.Migrations;

namespace UF5423_Aguas.Migrations
{
    public partial class AddUserImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(99)",
                maxLength: 99,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(99)",
                oldMaxLength: 99,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(99)",
                maxLength: 99,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(99)",
                oldMaxLength: 99);
        }
    }
}
