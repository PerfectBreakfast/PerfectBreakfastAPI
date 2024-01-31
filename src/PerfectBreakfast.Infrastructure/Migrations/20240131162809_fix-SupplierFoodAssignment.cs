using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectBreakfast.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixSupplierFoodAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierFoodAssignment_DailyOrder_DailyOrderId",
                table: "SupplierFoodAssignment");

            migrationBuilder.RenameColumn(
                name: "DailyOrderId",
                table: "SupplierFoodAssignment",
                newName: "PartnerId");

            migrationBuilder.RenameIndex(
                name: "IX_SupplierFoodAssignment_DailyOrderId",
                table: "SupplierFoodAssignment",
                newName: "IX_SupplierFoodAssignment_PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierFoodAssignment_Partner_PartnerId",
                table: "SupplierFoodAssignment",
                column: "PartnerId",
                principalTable: "Partner",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierFoodAssignment_Partner_PartnerId",
                table: "SupplierFoodAssignment");

            migrationBuilder.RenameColumn(
                name: "PartnerId",
                table: "SupplierFoodAssignment",
                newName: "DailyOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_SupplierFoodAssignment_PartnerId",
                table: "SupplierFoodAssignment",
                newName: "IX_SupplierFoodAssignment_DailyOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierFoodAssignment_DailyOrder_DailyOrderId",
                table: "SupplierFoodAssignment",
                column: "DailyOrderId",
                principalTable: "DailyOrder",
                principalColumn: "Id");
        }
    }
}
