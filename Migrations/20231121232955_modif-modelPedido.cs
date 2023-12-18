using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SushiPOP_YA1A_2C2023_G2.Migrations
{
    public partial class modifmodelPedido : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_PEDIDO_T_CARRITO_CarritoId1",
                table: "T_PEDIDO");

            migrationBuilder.DropForeignKey(
                name: "FK_T_PEDIDO_T_RECLAMO_CarritoId",
                table: "T_PEDIDO");

            migrationBuilder.DropIndex(
                name: "IX_T_PEDIDO_CarritoId1",
                table: "T_PEDIDO");

            migrationBuilder.DropColumn(
                name: "CarritoId1",
                table: "T_PEDIDO");

            migrationBuilder.CreateIndex(
                name: "IX_T_RECLAMO_PedidoId",
                table: "T_RECLAMO",
                column: "PedidoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_T_PEDIDO_T_CARRITO_CarritoId",
                table: "T_PEDIDO",
                column: "CarritoId",
                principalTable: "T_CARRITO",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_T_RECLAMO_T_PEDIDO_PedidoId",
                table: "T_RECLAMO",
                column: "PedidoId",
                principalTable: "T_PEDIDO",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_PEDIDO_T_CARRITO_CarritoId",
                table: "T_PEDIDO");

            migrationBuilder.DropForeignKey(
                name: "FK_T_RECLAMO_T_PEDIDO_PedidoId",
                table: "T_RECLAMO");

            migrationBuilder.DropIndex(
                name: "IX_T_RECLAMO_PedidoId",
                table: "T_RECLAMO");

            migrationBuilder.AddColumn<int>(
                name: "CarritoId1",
                table: "T_PEDIDO",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_T_PEDIDO_CarritoId1",
                table: "T_PEDIDO",
                column: "CarritoId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_T_PEDIDO_T_CARRITO_CarritoId1",
                table: "T_PEDIDO",
                column: "CarritoId1",
                principalTable: "T_CARRITO",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_T_PEDIDO_T_RECLAMO_CarritoId",
                table: "T_PEDIDO",
                column: "CarritoId",
                principalTable: "T_RECLAMO",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
