using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestApp.Migrations
{
    public partial class datum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DatumNarudzbine",
                table: "Narudzbine",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatumNarudzbine",
                table: "Narudzbine");
        }
    }
}
