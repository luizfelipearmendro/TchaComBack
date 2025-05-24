using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TchaComBack.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExtratoPontoModel_Funcionarios_Matricula",
                table: "ExtratoPontoModel");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Funcionarios_Matricula",
                table: "Usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExtratoPontoModel",
                table: "ExtratoPontoModel");

            migrationBuilder.RenameTable(
                name: "ExtratoPontoModel",
                newName: "ExtratosPonto");

            migrationBuilder.RenameIndex(
                name: "IX_ExtratoPontoModel_Matricula",
                table: "ExtratosPonto",
                newName: "IX_ExtratosPonto_Matricula");

            migrationBuilder.AlterColumn<int>(
                name: "Matricula",
                table: "Funcionarios",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 6,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExtratosPonto",
                table: "ExtratosPonto",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Funcionarios_Matricula",
                table: "Funcionarios",
                column: "Matricula",
                unique: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExtratosPonto_Funcionarios_Matricula",
                table: "ExtratosPonto");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Funcionarios_Matricula",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Funcionarios_Matricula",
                table: "Funcionarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExtratosPonto",
                table: "ExtratosPonto");

            migrationBuilder.RenameTable(
                name: "ExtratosPonto",
                newName: "ExtratoPontoModel");

            migrationBuilder.RenameIndex(
                name: "IX_ExtratosPonto_Matricula",
                table: "ExtratoPontoModel",
                newName: "IX_ExtratoPontoModel_Matricula");

            migrationBuilder.AlterColumn<int>(
                name: "Matricula",
                table: "Funcionarios",
                type: "int",
                maxLength: 6,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExtratoPontoModel",
                table: "ExtratoPontoModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExtratoPontoModel_Funcionarios_Matricula",
                table: "ExtratoPontoModel",
                column: "Matricula",
                principalTable: "Funcionarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Funcionarios_Matricula",
                table: "Usuarios",
                column: "Matricula",
                principalTable: "Funcionarios",
                principalColumn: "Id");
        }
    }
}
