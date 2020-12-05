using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TestApp.Migrations
{
    public partial class guidproizvodi2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Proizvodi",
                table: "Proizvodi");

            migrationBuilder.DropColumn(
                name: "Zika",
                table: "Proizvodi");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Proizvodi",
                nullable: false,
                defaultValue: Guid.NewGuid());

            migrationBuilder.AddPrimaryKey(
                name: "PK_Proizvodi",
                table: "Proizvodi",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Proizvodi",
                table: "Proizvodi");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Proizvodi");

            migrationBuilder.AddColumn<int>(
                name: "Zika",
                table: "Proizvodi",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Proizvodi",
                table: "Proizvodi",
                column: "Zika");
        }
    }
}
