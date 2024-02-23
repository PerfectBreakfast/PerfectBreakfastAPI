using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectBreakfast.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixrelationpaymentnethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethod_Order_OrderId",
                table: "PaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMethod_OrderId",
                table: "PaymentMethod");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "PaymentMethod");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentMethodId",
                table: "Order",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Order_PaymentMethodId",
                table: "Order",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_PaymentMethod_PaymentMethodId",
                table: "Order",
                column: "PaymentMethodId",
                principalTable: "PaymentMethod",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_PaymentMethod_PaymentMethodId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_PaymentMethodId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "Order");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "PaymentMethod",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethod_OrderId",
                table: "PaymentMethod",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethod_Order_OrderId",
                table: "PaymentMethod",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id");
        }
    }
}
