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
            migrationBuilder.AddColumn<int>(
                name: "EstadoCivilNavId",
                table: "Funcionarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RacaNavId",
                table: "Funcionarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Funcionarios_EstadoCivilNavId",
                table: "Funcionarios",
                column: "EstadoCivilNavId");

            migrationBuilder.CreateIndex(
                name: "IX_Funcionarios_RacaNavId",
                table: "Funcionarios",
                column: "RacaNavId");

            migrationBuilder.AddForeignKey(
                name: "FK_Funcionarios_EstadoCivil_EstadoCivilNavId",
                table: "Funcionarios",
                column: "EstadoCivilNavId",
                principalTable: "EstadoCivil",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Funcionarios_Raca_RacaNavId",
                table: "Funcionarios",
                column: "RacaNavId",
                principalTable: "Raca",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Funcionarios_EstadoCivil_EstadoCivilNavId",
                table: "Funcionarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Funcionarios_Raca_RacaNavId",
                table: "Funcionarios");

            migrationBuilder.DropIndex(
                name: "IX_Funcionarios_EstadoCivilNavId",
                table: "Funcionarios");

            migrationBuilder.DropIndex(
                name: "IX_Funcionarios_RacaNavId",
                table: "Funcionarios");

            migrationBuilder.DropColumn(
                name: "EstadoCivilNavId",
                table: "Funcionarios");

            migrationBuilder.DropColumn(
                name: "RacaNavId",
                table: "Funcionarios");
        }
    }
}
