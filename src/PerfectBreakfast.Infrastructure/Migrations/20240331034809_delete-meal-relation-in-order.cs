using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectBreakfast.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deletemealrelationinorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Meal_MealId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_MealId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "MealId",
                table: "Order");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MealId",
                table: "Order",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Order_MealId",
                table: "Order",
                column: "MealId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Meal_MealId",
                table: "Order",
                column: "MealId",
                principalTable: "Meal",
                principalColumn: "Id");
        }
    }
}
