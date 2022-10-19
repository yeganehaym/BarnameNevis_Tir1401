using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarnameNevis1401.Data.SqlServer.Migrations
{
    public partial class SpMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create proc [dbo].[USP_Add_tag]
@name nvarchar(max)
as
insert into Tags(Name) values(@name)");
            migrationBuilder.Sql(@"create PROC [dbo].[USP_FIND_TAGS] (@TAG_NAME NVARCHAR(MAX))
AS
SELECT ID,NAME,USERID FROM Tags 
WHERE Tags.Name LIKE '%' +@TAG_NAME+'%'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop proc USP_FIND_TAGS");
            migrationBuilder.Sql(@"drop proc USP_Add_tag");
        }
    }
}
