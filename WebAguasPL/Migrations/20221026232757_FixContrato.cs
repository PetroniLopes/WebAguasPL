using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAguasPL.Migrations
{
    public partial class FixContrato : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contrato_Clientes_ClienteID",
                table: "Contrato");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contrato",
                table: "Contrato");

            migrationBuilder.RenameTable(
                name: "Contrato",
                newName: "Contratos");

            migrationBuilder.RenameIndex(
                name: "IX_Contrato_ClienteID",
                table: "Contratos",
                newName: "IX_Contratos_ClienteID");

            migrationBuilder.AddColumn<DateTime>(
                name: "ContractDate",
                table: "Contratos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contratos",
                table: "Contratos",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_Clientes_ClienteID",
                table: "Contratos",
                column: "ClienteID",
                principalTable: "Clientes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_Clientes_ClienteID",
                table: "Contratos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contratos",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContractDate",
                table: "Contratos");

            migrationBuilder.RenameTable(
                name: "Contratos",
                newName: "Contrato");

            migrationBuilder.RenameIndex(
                name: "IX_Contratos_ClienteID",
                table: "Contrato",
                newName: "IX_Contrato_ClienteID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contrato",
                table: "Contrato",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Contrato_Clientes_ClienteID",
                table: "Contrato",
                column: "ClienteID",
                principalTable: "Clientes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
