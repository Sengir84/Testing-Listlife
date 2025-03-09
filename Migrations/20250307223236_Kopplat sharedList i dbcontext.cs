using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListLife.Migrations
{
    /// <inheritdoc />
    public partial class KopplatsharedListidbcontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedList_AspNetUsers_SharedWithUserId",
                table: "SharedList");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedList_ShoppingLists_ShoppingListId",
                table: "SharedList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SharedList",
                table: "SharedList");

            migrationBuilder.RenameTable(
                name: "SharedList",
                newName: "SharedLists");

            migrationBuilder.RenameIndex(
                name: "IX_SharedList_ShoppingListId",
                table: "SharedLists",
                newName: "IX_SharedLists_ShoppingListId");

            migrationBuilder.RenameIndex(
                name: "IX_SharedList_SharedWithUserId",
                table: "SharedLists",
                newName: "IX_SharedLists_SharedWithUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SharedLists",
                table: "SharedLists",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedLists_AspNetUsers_SharedWithUserId",
                table: "SharedLists",
                column: "SharedWithUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SharedLists_ShoppingLists_ShoppingListId",
                table: "SharedLists",
                column: "ShoppingListId",
                principalTable: "ShoppingLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedLists_AspNetUsers_SharedWithUserId",
                table: "SharedLists");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedLists_ShoppingLists_ShoppingListId",
                table: "SharedLists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SharedLists",
                table: "SharedLists");

            migrationBuilder.RenameTable(
                name: "SharedLists",
                newName: "SharedList");

            migrationBuilder.RenameIndex(
                name: "IX_SharedLists_ShoppingListId",
                table: "SharedList",
                newName: "IX_SharedList_ShoppingListId");

            migrationBuilder.RenameIndex(
                name: "IX_SharedLists_SharedWithUserId",
                table: "SharedList",
                newName: "IX_SharedList_SharedWithUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SharedList",
                table: "SharedList",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedList_AspNetUsers_SharedWithUserId",
                table: "SharedList",
                column: "SharedWithUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SharedList_ShoppingLists_ShoppingListId",
                table: "SharedList",
                column: "ShoppingListId",
                principalTable: "ShoppingLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
