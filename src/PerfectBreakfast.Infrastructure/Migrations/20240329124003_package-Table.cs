using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectBreakfast.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class packageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierFoodAssignment_DailyOrder_DailyOrderId",
                table: "SupplierFoodAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierFoodAssignment_Partner_PartnerId",
                table: "SupplierFoodAssignment");

            migrationBuilder.DropIndex(
                name: "IX_SupplierFoodAssignment_DailyOrderId",
                table: "SupplierFoodAssignment");

            migrationBuilder.DropColumn(
                name: "DailyOrderId",
                table: "SupplierFoodAssignment");

            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "SupplierFoodAssignment");

            migrationBuilder.RenameColumn(
                name: "PartnerId",
                table: "SupplierFoodAssignment",
                newName: "PackageId");

            migrationBuilder.RenameIndex(
                name: "IX_SupplierFoodAssignment_PartnerId",
                table: "SupplierFoodAssignment",
                newName: "IX_SupplierFoodAssignment_PackageId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDeadline",
                table: "SupplierFoodAssignment",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PackageId",
                table: "DailyOrder",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "Package",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TotalFoodAssignQuantity = table.Column<int>(type: "int", nullable: false),
                    MissingQuantity = table.Column<int>(type: "int", nullable: false),
                    SuccessfulQuantity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DailyOrderId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    PartnerId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
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
                    table.PrimaryKey("PK_Package", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Package_DailyOrder_DailyOrderId",
                        column: x => x.DailyOrderId,
                        principalTable: "DailyOrder",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Package_Partner_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partner",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Package_DailyOrderId",
                table: "Package",
                column: "DailyOrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Package_PartnerId",
                table: "Package",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierFoodAssignment_Package_PackageId",
                table: "SupplierFoodAssignment",
                column: "PackageId",
                principalTable: "Package",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierFoodAssignment_Package_PackageId",
                table: "SupplierFoodAssignment");

            migrationBuilder.DropTable(
                name: "Package");

            migrationBuilder.DropColumn(
                name: "DeliveryDeadline",
                table: "SupplierFoodAssignment");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "DailyOrder");

            migrationBuilder.RenameColumn(
                name: "PackageId",
                table: "SupplierFoodAssignment",
                newName: "PartnerId");

            migrationBuilder.RenameIndex(
                name: "IX_SupplierFoodAssignment_PackageId",
                table: "SupplierFoodAssignment",
                newName: "IX_SupplierFoodAssignment_PartnerId");

            migrationBuilder.AddColumn<Guid>(
                name: "DailyOrderId",
                table: "SupplierFoodAssignment",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Deadline",
                table: "SupplierFoodAssignment",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.CreateIndex(
                name: "IX_SupplierFoodAssignment_DailyOrderId",
                table: "SupplierFoodAssignment",
                column: "DailyOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierFoodAssignment_DailyOrder_DailyOrderId",
                table: "SupplierFoodAssignment",
                column: "DailyOrderId",
                principalTable: "DailyOrder",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierFoodAssignment_Partner_PartnerId",
                table: "SupplierFoodAssignment",
                column: "PartnerId",
                principalTable: "Partner",
                principalColumn: "Id");
        }
    }
}
