using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectBreakfast.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addindexname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Supplier_Name",
                table: "Supplier",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Partner_Name",
                table: "Partner",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Company_Name",
                table: "Company",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Supplier_Name",
                table: "Supplier");

            migrationBuilder.DropIndex(
                name: "IX_Partner_Name",
                table: "Partner");

            migrationBuilder.DropIndex(
                name: "IX_Company_Name",
                table: "Company");

            migrationBuilder.CreateIndex(
                name: "IX_Company_Name",
                table: "Company",
                column: "Name");
        }
    }
}
