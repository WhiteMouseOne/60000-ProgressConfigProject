using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Progress.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AlignPdfSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "WorkpieceOrderLines",
                newName: "CreateTime");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "WorkpieceOrderLines",
                newName: "UpdateTime");

            migrationBuilder.AddColumn<string>(
                name: "Material",
                table: "WorkpieceOrderLines",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "ReceivedQuantity",
                table: "WorkpieceOrderLines",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippedQuantity",
                table: "WorkpieceOrderLines",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VendorEstimatedDeliveryDate",
                table: "WorkpieceOrderLines",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateBy",
                table: "Users",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "HeadPortrait",
                table: "Users",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "Token",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateBy",
                table: "Users",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.RenameColumn(
                name: "Contact",
                table: "Suppliers",
                newName: "Contact1Name");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Suppliers",
                newName: "Contact1Phone");

            migrationBuilder.AddColumn<string>(
                name: "Account",
                table: "Suppliers",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Suppliers",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Contact1Email",
                table: "Suppliers",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Contact2Name",
                table: "Suppliers",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Contact2Phone",
                table: "Suppliers",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Contact2Email",
                table: "Suppliers",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "SupplierNumber",
                table: "Suppliers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Enable",
                table: "Roles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateBy",
                table: "Roles",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Roles",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoleSort",
                table: "Roles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UpdateBy",
                table: "Roles",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateTime",
                table: "Roles",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Enable",
                table: "Menus",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateBy",
                table: "Menus",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UpdateBy",
                table: "Menus",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateTime",
                table: "Menus",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CraftRecipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftRecipes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CraftRecipeSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CraftRecipeId = table.Column<int>(type: "int", nullable: false),
                    CraftId = table.Column<int>(type: "int", nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftRecipeSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CraftRecipeSteps_CraftRecipes_CraftRecipeId",
                        column: x => x.CraftRecipeId,
                        principalTable: "CraftRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CraftRecipeSteps_Crafts_CraftId",
                        column: x => x.CraftId,
                        principalTable: "Crafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_SupplierNumber",
                table: "Suppliers",
                column: "SupplierNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CraftRecipes_Code",
                table: "CraftRecipes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CraftRecipeSteps_CraftId",
                table: "CraftRecipeSteps",
                column: "CraftId");

            migrationBuilder.CreateIndex(
                name: "IX_CraftRecipeSteps_CraftRecipeId",
                table: "CraftRecipeSteps",
                column: "CraftRecipeId");

            migrationBuilder.Sql("UPDATE Suppliers SET SupplierNumber = Id WHERE SupplierNumber = 0;");
            migrationBuilder.Sql("UPDATE WorkpieceOrderLines SET ShippingStatus = 2 WHERE ShippingStatus = 0;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CraftRecipeSteps");

            migrationBuilder.DropTable(
                name: "CraftRecipes");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_SupplierNumber",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Material",
                table: "WorkpieceOrderLines");

            migrationBuilder.DropColumn(
                name: "ReceivedQuantity",
                table: "WorkpieceOrderLines");

            migrationBuilder.DropColumn(
                name: "ShippedQuantity",
                table: "WorkpieceOrderLines");

            migrationBuilder.DropColumn(
                name: "VendorEstimatedDeliveryDate",
                table: "WorkpieceOrderLines");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HeadPortrait",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdateBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Account",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Contact1Email",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Contact2Name",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Contact2Phone",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Contact2Email",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "SupplierNumber",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "RoleSort",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "UpdateBy",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "UpdateTime",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "UpdateBy",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "UpdateTime",
                table: "Menus");

            migrationBuilder.RenameColumn(
                name: "CreateTime",
                table: "WorkpieceOrderLines",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdateTime",
                table: "WorkpieceOrderLines",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "Contact1Name",
                table: "Suppliers",
                newName: "Contact");

            migrationBuilder.RenameColumn(
                name: "Contact1Phone",
                table: "Suppliers",
                newName: "Phone");

            migrationBuilder.AlterColumn<int>(
                name: "Enable",
                table: "Roles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Enable",
                table: "Menus",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.Sql("UPDATE WorkpieceOrderLines SET ShippingStatus = 0 WHERE ShippingStatus = 2;");
        }
    }
}
