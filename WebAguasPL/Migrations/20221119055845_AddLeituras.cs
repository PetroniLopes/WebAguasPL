using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAguasPL.Migrations
{
    public partial class AddLeituras : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leituras",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataLeitura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Valor = table.Column<double>(type: "float", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    ContratoID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leituras", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Leituras_Contratos_ContratoID",
                        column: x => x.ContratoID,
                        principalTable: "Contratos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Leituras_ContratoID",
                table: "Leituras",
                column: "ContratoID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Leituras");
        }
    }
}
