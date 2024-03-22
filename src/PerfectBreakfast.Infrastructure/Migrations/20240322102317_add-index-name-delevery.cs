using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectBreakfast.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addindexnamedelevery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Delivery_Name",
                table: "Delivery",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Delivery_Name",
                table: "Delivery");
        }
    }
}
