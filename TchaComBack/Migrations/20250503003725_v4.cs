using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TchaComBack.Migrations
{
    /// <inheritdoc />
    public partial class v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Funcionarios_EstadoCivil_EstadoCivilNavId",
                table: "Funcionarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Funcionarios_Raca_RacaNavId",
                table: "Funcionarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Funcionarios_Setores_SetorId",
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

            migrationBuilder.CreateIndex(
                name: "IX_Setores_CategoriaId",
                table: "Setores",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Funcionarios_EstadoCivil",
                table: "Funcionarios",
                column: "EstadoCivil");

            migrationBuilder.CreateIndex(
                name: "IX_Funcionarios_Raca",
                table: "Funcionarios",
                column: "Raca");

            migrationBuilder.AddForeignKey(
                name: "FK_Funcionarios_EstadoCivil_EstadoCivil",
                table: "Funcionarios",
                column: "EstadoCivil",
                principalTable: "EstadoCivil",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Funcionarios_Raca_Raca",
                table: "Funcionarios",
                column: "Raca",
                principalTable: "Raca",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Funcionarios_Setores_SetorId",
                table: "Funcionarios",
                column: "SetorId",
                principalTable: "Setores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Setores_Categorias_CategoriaId",
                table: "Setores",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Funcionarios_EstadoCivil_EstadoCivil",
                table: "Funcionarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Funcionarios_Raca_Raca",
                table: "Funcionarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Funcionarios_Setores_SetorId",
                table: "Funcionarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Setores_Categorias_CategoriaId",
                table: "Setores");

            migrationBuilder.DropIndex(
                name: "IX_Setores_CategoriaId",
                table: "Setores");

            migrationBuilder.DropIndex(
                name: "IX_Funcionarios_EstadoCivil",
                table: "Funcionarios");

            migrationBuilder.DropIndex(
                name: "IX_Funcionarios_Raca",
                table: "Funcionarios");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Funcionarios_Setores_SetorId",
                table: "Funcionarios",
                column: "SetorId",
                principalTable: "Setores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
