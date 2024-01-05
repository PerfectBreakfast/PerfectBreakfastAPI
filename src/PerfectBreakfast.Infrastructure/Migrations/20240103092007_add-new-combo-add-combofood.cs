using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectBreakfast.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addnewcomboaddcombofood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuFood_Food_FoodId",
                table: "MenuFood");

            migrationBuilder.AddColumn<Guid>(
                name: "ComboId",
                table: "OrderDetail",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "FoodId",
                table: "MenuFood",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "ComboId",
                table: "MenuFood",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartWorkHour",
                table: "Company",
                type: "time(6)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Combo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
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
                    table.PrimaryKey("PK_Combo", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ComboFood",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ComboId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    FoodId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
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
                    table.PrimaryKey("PK_ComboFood", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComboFood_Combo_ComboId",
                        column: x => x.ComboId,
                        principalTable: "Combo",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ComboFood_Food_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Food",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ComboId",
                table: "OrderDetail",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuFood_ComboId",
                table: "MenuFood",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboFood_ComboId",
                table: "ComboFood",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboFood_FoodId",
                table: "ComboFood",
                column: "FoodId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuFood_Combo_ComboId",
                table: "MenuFood",
                column: "ComboId",
                principalTable: "Combo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuFood_Food_FoodId",
                table: "MenuFood",
                column: "FoodId",
                principalTable: "Food",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Combo_ComboId",
                table: "OrderDetail",
                column: "ComboId",
                principalTable: "Combo",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuFood_Combo_ComboId",
                table: "MenuFood");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuFood_Food_FoodId",
                table: "MenuFood");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Combo_ComboId",
                table: "OrderDetail");

            migrationBuilder.DropTable(
                name: "ComboFood");

            migrationBuilder.DropTable(
                name: "Combo");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetail_ComboId",
                table: "OrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_MenuFood_ComboId",
                table: "MenuFood");

            migrationBuilder.DropColumn(
                name: "ComboId",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "ComboId",
                table: "MenuFood");

            migrationBuilder.DropColumn(
                name: "StartWorkHour",
                table: "Company");

            migrationBuilder.AlterColumn<Guid>(
                name: "FoodId",
                table: "MenuFood",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuFood_Food_FoodId",
                table: "MenuFood",
                column: "FoodId",
                principalTable: "Food",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
