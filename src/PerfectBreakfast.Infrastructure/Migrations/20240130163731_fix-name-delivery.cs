using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectBreakfast.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixnamedelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Company_DeliveryUnit_DeliveryId",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerPayment_DeliveryUnit_DeliveryId",
                table: "PartnerPayment");

            migrationBuilder.DropForeignKey(
                name: "FK_User_DeliveryUnit_DeliveryId",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveryUnit",
                table: "DeliveryUnit");

            migrationBuilder.RenameTable(
                name: "DeliveryUnit",
                newName: "Delivery");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Delivery",
                table: "Delivery",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Delivery_DeliveryId",
                table: "Company",
                column: "DeliveryId",
                principalTable: "Delivery",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerPayment_Delivery_DeliveryId",
                table: "PartnerPayment",
                column: "DeliveryId",
                principalTable: "Delivery",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Delivery_DeliveryId",
                table: "User",
                column: "DeliveryId",
                principalTable: "Delivery",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Company_Delivery_DeliveryId",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerPayment_Delivery_DeliveryId",
                table: "PartnerPayment");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Delivery_DeliveryId",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Delivery",
                table: "Delivery");

            migrationBuilder.RenameTable(
                name: "Delivery",
                newName: "DeliveryUnit");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveryUnit",
                table: "DeliveryUnit",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_DeliveryUnit_DeliveryId",
                table: "Company",
                column: "DeliveryId",
                principalTable: "DeliveryUnit",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerPayment_DeliveryUnit_DeliveryId",
                table: "PartnerPayment",
                column: "DeliveryId",
                principalTable: "DeliveryUnit",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_DeliveryUnit_DeliveryId",
                table: "User",
                column: "DeliveryId",
                principalTable: "DeliveryUnit",
                principalColumn: "Id");
        }
    }
}
