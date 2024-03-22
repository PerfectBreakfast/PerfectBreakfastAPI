using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectBreakfast.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addindexmenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Menu_IsSelected",
                table: "Menu",
                column: "IsSelected");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Menu_IsSelected",
                table: "Menu");
        }
    }
}
