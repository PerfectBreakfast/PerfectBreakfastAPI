using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectBreakfast.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixdailyOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyOrder_Company_CompanyId",
                table: "DailyOrder");

            migrationBuilder.DropIndex(
                name: "IX_DailyOrder_CompanyId",
                table: "DailyOrder");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "DailyOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "DailyOrder",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_DailyOrder_CompanyId",
                table: "DailyOrder",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyOrder_Company_CompanyId",
                table: "DailyOrder",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id");
        }
    }
}
