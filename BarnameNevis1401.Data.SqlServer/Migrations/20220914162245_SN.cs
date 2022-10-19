using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarnameNevis1401.Data.SqlServer.Migrations
{
    public partial class SN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "Users");
        }
    }
}
