using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListLife.Migrations
{
    /// <inheritdoc />
    public partial class listtouserconnectionfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingLists_Categories_CategoryId",
                table: "ShoppingLists");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingLists_CategoryId",
                table: "ShoppingLists");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ShoppingLists");

            migrationBuilder.DropColumn(
                name: "ShoppingId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "ShoppingLists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "ShoppingLists");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "ShoppingLists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShoppingId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingLists_CategoryId",
                table: "ShoppingLists",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingLists_Categories_CategoryId",
                table: "ShoppingLists",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
