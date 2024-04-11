using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectBreakfast.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Package");

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderStatus",
                table: "Order",
                column: "OrderStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Order_OrderStatus",
                table: "Order");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Package",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
