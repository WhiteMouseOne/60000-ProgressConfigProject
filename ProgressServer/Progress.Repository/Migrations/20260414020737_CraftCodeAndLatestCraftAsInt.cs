using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Progress.Repository.Migrations
{
    /// <inheritdoc />
    public partial class CraftCodeAndLatestCraftAsInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 存量 varchar 编码转为可写入 int 的数值（种子 C1 -> 1；其它可按需扩展）
            migrationBuilder.Sql("UPDATE `Crafts` SET `Code` = '1' WHERE `Code` = 'C1';");
            migrationBuilder.Sql("UPDATE `WorkpieceOrderLines` SET `LatestCraftCode` = '1' WHERE `LatestCraftCode` = 'C1';");

            migrationBuilder.AlterColumn<int>(
                name: "LatestCraftCode",
                table: "WorkpieceOrderLines",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Code",
                table: "Crafts",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64)
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LatestCraftCode",
                table: "WorkpieceOrderLines",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Crafts",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
