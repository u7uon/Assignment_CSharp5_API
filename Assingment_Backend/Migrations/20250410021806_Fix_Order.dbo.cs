﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assingment_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Orderdbo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Order_OrderId",
                table: "OrderDetail");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "OrderDetail",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Order_OrderId",
                table: "OrderDetail",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Order_OrderId",
                table: "OrderDetail");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "OrderDetail",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Order_OrderId",
                table: "OrderDetail",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "OrderId");
        }
    }
}
