using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectBreakfast.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixrelationorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Partner_PartnerId",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "PartnerId",
                table: "Order",
                newName: "DeliveryStaffId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_PartnerId",
                table: "Order",
                newName: "IX_Order_DeliveryStaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_User_DeliveryStaffId",
                table: "Order",
                column: "DeliveryStaffId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_User_DeliveryStaffId",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "DeliveryStaffId",
                table: "Order",
                newName: "PartnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_DeliveryStaffId",
                table: "Order",
                newName: "IX_Order_PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Partner_PartnerId",
                table: "Order",
                column: "PartnerId",
                principalTable: "Partner",
                principalColumn: "Id");
        }
    }
}
