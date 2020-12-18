using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestApp.Migrations
{
    public partial class narudzbina : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Narudzbine",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    NarucilacId = table.Column<string>(nullable: true),
                    PrimalacId = table.Column<string>(nullable: true),
                    StatusNarudzbine = table.Column<int>(nullable: false),
                    VremeIsporukeUDanima = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Narudzbine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Narudzbine_AspNetUsers_NarucilacId",
                        column: x => x.NarucilacId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Narudzbine_AspNetUsers_PrimalacId",
                        column: x => x.PrimalacId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ElementKorpe",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ProizvodId = table.Column<Guid>(nullable: true),
                    Kolicina = table.Column<int>(nullable: false),
                    NarudzbinaId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementKorpe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElementKorpe_Narudzbine_NarudzbinaId",
                        column: x => x.NarudzbinaId,
                        principalTable: "Narudzbine",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ElementKorpe_Proizvodi_ProizvodId",
                        column: x => x.ProizvodId,
                        principalTable: "Proizvodi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElementKorpe_NarudzbinaId",
                table: "ElementKorpe",
                column: "NarudzbinaId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementKorpe_ProizvodId",
                table: "ElementKorpe",
                column: "ProizvodId");

            migrationBuilder.CreateIndex(
                name: "IX_Narudzbine_NarucilacId",
                table: "Narudzbine",
                column: "NarucilacId");

            migrationBuilder.CreateIndex(
                name: "IX_Narudzbine_PrimalacId",
                table: "Narudzbine",
                column: "PrimalacId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElementKorpe");

            migrationBuilder.DropTable(
                name: "Narudzbine");
        }
    }
}
