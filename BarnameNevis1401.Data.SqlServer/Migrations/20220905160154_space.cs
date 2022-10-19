using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarnameNevis1401.Data.SqlServer.Migrations
{
    public partial class space : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Space",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Space",
                table: "Users");
        }
    }
}
