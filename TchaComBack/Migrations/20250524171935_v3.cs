using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TchaComBack.Migrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExtratosPonto_Funcionarios_Matricula",
                table: "ExtratosPonto");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Funcionarios_Matricula",
                table: "Usuarios");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Funcionarios_Matricula",
                table: "Funcionarios",
                column: "Matricula");

            migrationBuilder.AddForeignKey(
                name: "FK_ExtratosPonto_Funcionarios_Matricula",
                table: "ExtratosPonto",
                column: "Matricula",
                principalTable: "Funcionarios",
                principalColumn: "Matricula",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Funcionarios_Matricula",
                table: "Usuarios",
                column: "Matricula",
                principalTable: "Funcionarios",
                principalColumn: "Matricula",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExtratosPonto_Funcionarios_Matricula",
                table: "ExtratosPonto");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Funcionarios_Matricula",
                table: "Usuarios");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Funcionarios_Matricula",
                table: "Funcionarios");

            migrationBuilder.AddForeignKey(
                name: "FK_ExtratosPonto_Funcionarios_Matricula",
                table: "ExtratosPonto",
                column: "Matricula",
                principalTable: "Funcionarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Funcionarios_Matricula",
                table: "Usuarios",
                column: "Matricula",
                principalTable: "Funcionarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
