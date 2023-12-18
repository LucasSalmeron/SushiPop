using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SushiPOP_YA1A_2C2023_G2.Migrations
{
    public partial class modifmodeldescuento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "T_USUARIO",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "T_USUARIO",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
