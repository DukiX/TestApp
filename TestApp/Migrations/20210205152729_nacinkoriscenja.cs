using Microsoft.EntityFrameworkCore.Migrations;

namespace TestApp.Migrations
{
    public partial class nacinkoriscenja : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NacinKoriscenja",
                table: "Proizvodi",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NacinKoriscenja",
                table: "Proizvodi",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
