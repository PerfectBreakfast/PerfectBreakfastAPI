using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectBreakfast.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatebig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuFood_Combo_ComboId",
                table: "MenuFood");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_DeliveryUnit_DeliveryUnitId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Supplier_SupplierId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_User_ShipperId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderHistory_Order_OrderId",
                table: "OrderHistory");

            migrationBuilder.DropTable(
                name: "DeliveryAssignment");

            migrationBuilder.DropIndex(
                name: "IX_Order_DeliveryUnitId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_ShipperId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DeliveryUnitId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShipperId",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "OrderHistory",
                newName: "DailyOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderHistory_OrderId",
                table: "OrderHistory",
                newName: "IX_OrderHistory_DailyOrderId");

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                table: "Order",
                newName: "DailyOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_SupplierId",
                table: "Order",
                newName: "IX_Order_DailyOrderId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ComboId",
                table: "MenuFood",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "CommissionRate",
                table: "ManagementUnit",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryUnitId",
                table: "Company",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "ManagementUnitId",
                table: "Company",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "DailyOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OrderQuantity = table.Column<int>(type: "int", nullable: true),
                    BookingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    AdminId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModificationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModificationBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DeletionDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeleteBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyOrder_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DailyOrder_User_AdminId",
                        column: x => x.AdminId,
                        principalTable: "User",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SupplierCommissionRate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CommissionRate = table.Column<int>(type: "int", nullable: false),
                    FoodId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    SupplierId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModificationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModificationBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DeletionDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeleteBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierCommissionRate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierCommissionRate_Food_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Food",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplierCommissionRate_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PartnerPayment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RemittanceTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DailyOrderId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    SupperAdminId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DeliveryUnitId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ManagementUnitId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    SupplierId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModificationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModificationBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DeletionDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeleteBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnerPayment_DailyOrder_DailyOrderId",
                        column: x => x.DailyOrderId,
                        principalTable: "DailyOrder",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerPayment_DeliveryUnit_DeliveryUnitId",
                        column: x => x.DeliveryUnitId,
                        principalTable: "DeliveryUnit",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerPayment_ManagementUnit_ManagementUnitId",
                        column: x => x.ManagementUnitId,
                        principalTable: "ManagementUnit",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerPayment_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerPayment_User_SupperAdminId",
                        column: x => x.SupperAdminId,
                        principalTable: "User",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ShippingOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DailyOrderId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ShipperId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModificationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModificationBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DeletionDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeleteBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingOrder_DailyOrder_DailyOrderId",
                        column: x => x.DailyOrderId,
                        principalTable: "DailyOrder",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShippingOrder_User_ShipperId",
                        column: x => x.ShipperId,
                        principalTable: "User",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SupplierFoodAssignment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DateCooked = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AmountCooked = table.Column<int>(type: "int", nullable: false),
                    ReceivedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FoodId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    SupplierCommissionRateId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DailyOrderId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModificationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModificationBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DeletionDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeleteBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierFoodAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierFoodAssignment_DailyOrder_DailyOrderId",
                        column: x => x.DailyOrderId,
                        principalTable: "DailyOrder",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplierFoodAssignment_Food_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Food",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplierFoodAssignment_SupplierCommissionRate_SupplierCommis~",
                        column: x => x.SupplierCommissionRateId,
                        principalTable: "SupplierCommissionRate",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Company_DeliveryUnitId",
                table: "Company",
                column: "DeliveryUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_ManagementUnitId",
                table: "Company",
                column: "ManagementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyOrder_AdminId",
                table: "DailyOrder",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyOrder_CompanyId",
                table: "DailyOrder",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerPayment_DailyOrderId",
                table: "PartnerPayment",
                column: "DailyOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerPayment_DeliveryUnitId",
                table: "PartnerPayment",
                column: "DeliveryUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerPayment_ManagementUnitId",
                table: "PartnerPayment",
                column: "ManagementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerPayment_SupperAdminId",
                table: "PartnerPayment",
                column: "SupperAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerPayment_SupplierId",
                table: "PartnerPayment",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingOrder_DailyOrderId",
                table: "ShippingOrder",
                column: "DailyOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingOrder_ShipperId",
                table: "ShippingOrder",
                column: "ShipperId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCommissionRate_FoodId",
                table: "SupplierCommissionRate",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCommissionRate_SupplierId",
                table: "SupplierCommissionRate",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierFoodAssignment_DailyOrderId",
                table: "SupplierFoodAssignment",
                column: "DailyOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierFoodAssignment_FoodId",
                table: "SupplierFoodAssignment",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierFoodAssignment_SupplierCommissionRateId",
                table: "SupplierFoodAssignment",
                column: "SupplierCommissionRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_DeliveryUnit_DeliveryUnitId",
                table: "Company",
                column: "DeliveryUnitId",
                principalTable: "DeliveryUnit",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_ManagementUnit_ManagementUnitId",
                table: "Company",
                column: "ManagementUnitId",
                principalTable: "ManagementUnit",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuFood_Combo_ComboId",
                table: "MenuFood",
                column: "ComboId",
                principalTable: "Combo",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_DailyOrder_DailyOrderId",
                table: "Order",
                column: "DailyOrderId",
                principalTable: "DailyOrder",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHistory_DailyOrder_DailyOrderId",
                table: "OrderHistory",
                column: "DailyOrderId",
                principalTable: "DailyOrder",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Company_DeliveryUnit_DeliveryUnitId",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "FK_Company_ManagementUnit_ManagementUnitId",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuFood_Combo_ComboId",
                table: "MenuFood");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_DailyOrder_DailyOrderId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderHistory_DailyOrder_DailyOrderId",
                table: "OrderHistory");

            migrationBuilder.DropTable(
                name: "PartnerPayment");

            migrationBuilder.DropTable(
                name: "ShippingOrder");

            migrationBuilder.DropTable(
                name: "SupplierFoodAssignment");

            migrationBuilder.DropTable(
                name: "DailyOrder");

            migrationBuilder.DropTable(
                name: "SupplierCommissionRate");

            migrationBuilder.DropIndex(
                name: "IX_Company_DeliveryUnitId",
                table: "Company");

            migrationBuilder.DropIndex(
                name: "IX_Company_ManagementUnitId",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "CommissionRate",
                table: "ManagementUnit");

            migrationBuilder.DropColumn(
                name: "DeliveryUnitId",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "ManagementUnitId",
                table: "Company");

            migrationBuilder.RenameColumn(
                name: "DailyOrderId",
                table: "OrderHistory",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderHistory_DailyOrderId",
                table: "OrderHistory",
                newName: "IX_OrderHistory_OrderId");

            migrationBuilder.RenameColumn(
                name: "DailyOrderId",
                table: "Order",
                newName: "SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_DailyOrderId",
                table: "Order",
                newName: "IX_Order_SupplierId");

            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryUnitId",
                table: "Order",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "ShipperId",
                table: "Order",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "ComboId",
                table: "MenuFood",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "DeliveryAssignment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DeliveryUnitId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ManagementUnitId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DeleteBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DeletionDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModificationBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModificationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryAssignment_DeliveryUnit_DeliveryUnitId",
                        column: x => x.DeliveryUnitId,
                        principalTable: "DeliveryUnit",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DeliveryAssignment_ManagementUnit_ManagementUnitId",
                        column: x => x.ManagementUnitId,
                        principalTable: "ManagementUnit",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Order_DeliveryUnitId",
                table: "Order",
                column: "DeliveryUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_ShipperId",
                table: "Order",
                column: "ShipperId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAssignment_DeliveryUnitId",
                table: "DeliveryAssignment",
                column: "DeliveryUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAssignment_ManagementUnitId",
                table: "DeliveryAssignment",
                column: "ManagementUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuFood_Combo_ComboId",
                table: "MenuFood",
                column: "ComboId",
                principalTable: "Combo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_DeliveryUnit_DeliveryUnitId",
                table: "Order",
                column: "DeliveryUnitId",
                principalTable: "DeliveryUnit",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Supplier_SupplierId",
                table: "Order",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_User_ShipperId",
                table: "Order",
                column: "ShipperId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHistory_Order_OrderId",
                table: "OrderHistory",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id");
        }
    }
}
