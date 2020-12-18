using Microsoft.EntityFrameworkCore.Migrations;

namespace TestApp.Migrations
{
    public partial class narudzbinaUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Narudzbine_AspNetUsers_NarucilacId",
                table: "Narudzbine");

            migrationBuilder.DropForeignKey(
                name: "FK_Narudzbine_AspNetUsers_PrimalacId",
                table: "Narudzbine");

            migrationBuilder.DropIndex(
                name: "IX_Narudzbine_NarucilacId",
                table: "Narudzbine");

            migrationBuilder.DropIndex(
                name: "IX_Narudzbine_PrimalacId",
                table: "Narudzbine");

            migrationBuilder.DropColumn(
                name: "NarucilacId",
                table: "Narudzbine");

            migrationBuilder.DropColumn(
                name: "PrimalacId",
                table: "Narudzbine");

            migrationBuilder.AddColumn<string>(
                name: "KupacId",
                table: "Narudzbine",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProdavacId",
                table: "Narudzbine",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Narudzbine_KupacId",
                table: "Narudzbine",
                column: "KupacId");

            migrationBuilder.CreateIndex(
                name: "IX_Narudzbine_ProdavacId",
                table: "Narudzbine",
                column: "ProdavacId");

            migrationBuilder.AddForeignKey(
                name: "FK_Narudzbine_AspNetUsers_KupacId",
                table: "Narudzbine",
                column: "KupacId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Narudzbine_AspNetUsers_ProdavacId",
                table: "Narudzbine",
                column: "ProdavacId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Narudzbine_AspNetUsers_KupacId",
                table: "Narudzbine");

            migrationBuilder.DropForeignKey(
                name: "FK_Narudzbine_AspNetUsers_ProdavacId",
                table: "Narudzbine");

            migrationBuilder.DropIndex(
                name: "IX_Narudzbine_KupacId",
                table: "Narudzbine");

            migrationBuilder.DropIndex(
                name: "IX_Narudzbine_ProdavacId",
                table: "Narudzbine");

            migrationBuilder.DropColumn(
                name: "KupacId",
                table: "Narudzbine");

            migrationBuilder.DropColumn(
                name: "ProdavacId",
                table: "Narudzbine");

            migrationBuilder.AddColumn<string>(
                name: "NarucilacId",
                table: "Narudzbine",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimalacId",
                table: "Narudzbine",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Narudzbine_NarucilacId",
                table: "Narudzbine",
                column: "NarucilacId");

            migrationBuilder.CreateIndex(
                name: "IX_Narudzbine_PrimalacId",
                table: "Narudzbine",
                column: "PrimalacId");

            migrationBuilder.AddForeignKey(
                name: "FK_Narudzbine_AspNetUsers_NarucilacId",
                table: "Narudzbine",
                column: "NarucilacId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Narudzbine_AspNetUsers_PrimalacId",
                table: "Narudzbine",
                column: "PrimalacId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
