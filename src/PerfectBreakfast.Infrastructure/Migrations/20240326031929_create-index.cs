using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectBreakfast.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class createindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Supplier_IsDeleted",
                table: "Supplier",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingOrder_IsDeleted",
                table: "ShippingOrder",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Partner_IsDeleted",
                table: "Partner",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_MenuFood_IsDeleted",
                table: "MenuFood",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Menu_IsDeleted",
                table: "Menu",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Food_IsDeleted",
                table: "Food",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_IsDeleted",
                table: "Delivery",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_DailyOrder_IsDeleted",
                table: "DailyOrder",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Company_IsDeleted",
                table: "Company",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ComboFood_IsDeleted",
                table: "ComboFood",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Combo_IsDeleted",
                table: "Combo",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Supplier_IsDeleted",
                table: "Supplier");

            migrationBuilder.DropIndex(
                name: "IX_ShippingOrder_IsDeleted",
                table: "ShippingOrder");

            migrationBuilder.DropIndex(
                name: "IX_Partner_IsDeleted",
                table: "Partner");

            migrationBuilder.DropIndex(
                name: "IX_MenuFood_IsDeleted",
                table: "MenuFood");

            migrationBuilder.DropIndex(
                name: "IX_Menu_IsDeleted",
                table: "Menu");

            migrationBuilder.DropIndex(
                name: "IX_Food_IsDeleted",
                table: "Food");

            migrationBuilder.DropIndex(
                name: "IX_Delivery_IsDeleted",
                table: "Delivery");

            migrationBuilder.DropIndex(
                name: "IX_DailyOrder_IsDeleted",
                table: "DailyOrder");

            migrationBuilder.DropIndex(
                name: "IX_Company_IsDeleted",
                table: "Company");

            migrationBuilder.DropIndex(
                name: "IX_ComboFood_IsDeleted",
                table: "ComboFood");

            migrationBuilder.DropIndex(
                name: "IX_Combo_IsDeleted",
                table: "Combo");
        }
    }
}
