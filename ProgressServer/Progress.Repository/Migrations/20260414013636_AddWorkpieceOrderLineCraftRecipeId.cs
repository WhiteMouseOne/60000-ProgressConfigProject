using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Progress.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkpieceOrderLineCraftRecipeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CraftRecipeId",
                table: "WorkpieceOrderLines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkpieceOrderLines_CraftRecipeId",
                table: "WorkpieceOrderLines",
                column: "CraftRecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkpieceOrderLines_CraftRecipes_CraftRecipeId",
                table: "WorkpieceOrderLines",
                column: "CraftRecipeId",
                principalTable: "CraftRecipes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkpieceOrderLines_CraftRecipes_CraftRecipeId",
                table: "WorkpieceOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_WorkpieceOrderLines_CraftRecipeId",
                table: "WorkpieceOrderLines");

            migrationBuilder.DropColumn(
                name: "CraftRecipeId",
                table: "WorkpieceOrderLines");
        }
    }
}
