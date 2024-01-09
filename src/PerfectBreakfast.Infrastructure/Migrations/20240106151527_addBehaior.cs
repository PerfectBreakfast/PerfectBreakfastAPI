using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectBreakfast.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addBehaior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComboFood_Combo_ComboId",
                table: "ComboFood");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Order_OrderId",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierCommissionRate_Food_FoodId",
                table: "SupplierCommissionRate");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierCommissionRate_Supplier_SupplierId",
                table: "SupplierCommissionRate");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplyAssignment_ManagementUnit_ManagementUnitId",
                table: "SupplyAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplyAssignment_Supplier_SupplierId",
                table: "SupplyAssignment");

            migrationBuilder.AddForeignKey(
                name: "FK_ComboFood_Combo_ComboId",
                table: "ComboFood",
                column: "ComboId",
                principalTable: "Combo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Order_OrderId",
                table: "OrderDetail",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierCommissionRate_Food_FoodId",
                table: "SupplierCommissionRate",
                column: "FoodId",
                principalTable: "Food",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierCommissionRate_Supplier_SupplierId",
                table: "SupplierCommissionRate",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplyAssignment_ManagementUnit_ManagementUnitId",
                table: "SupplyAssignment",
                column: "ManagementUnitId",
                principalTable: "ManagementUnit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplyAssignment_Supplier_SupplierId",
                table: "SupplyAssignment",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComboFood_Combo_ComboId",
                table: "ComboFood");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Order_OrderId",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierCommissionRate_Food_FoodId",
                table: "SupplierCommissionRate");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierCommissionRate_Supplier_SupplierId",
                table: "SupplierCommissionRate");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplyAssignment_ManagementUnit_ManagementUnitId",
                table: "SupplyAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplyAssignment_Supplier_SupplierId",
                table: "SupplyAssignment");

            migrationBuilder.AddForeignKey(
                name: "FK_ComboFood_Combo_ComboId",
                table: "ComboFood",
                column: "ComboId",
                principalTable: "Combo",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Order_OrderId",
                table: "OrderDetail",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierCommissionRate_Food_FoodId",
                table: "SupplierCommissionRate",
                column: "FoodId",
                principalTable: "Food",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierCommissionRate_Supplier_SupplierId",
                table: "SupplierCommissionRate",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplyAssignment_ManagementUnit_ManagementUnitId",
                table: "SupplyAssignment",
                column: "ManagementUnitId",
                principalTable: "ManagementUnit",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplyAssignment_Supplier_SupplierId",
                table: "SupplyAssignment",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "Id");
        }
    }
}
