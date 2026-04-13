using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Progress.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderLineRepairFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RepairCreatedAt",
                table: "WorkpieceOrderLines",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RepairShippedAt",
                table: "WorkpieceOrderLines",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RepairStartedAt",
                table: "WorkpieceOrderLines",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RepairStatus",
                table: "WorkpieceOrderLines",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepairCreatedAt",
                table: "WorkpieceOrderLines");

            migrationBuilder.DropColumn(
                name: "RepairShippedAt",
                table: "WorkpieceOrderLines");

            migrationBuilder.DropColumn(
                name: "RepairStartedAt",
                table: "WorkpieceOrderLines");

            migrationBuilder.DropColumn(
                name: "RepairStatus",
                table: "WorkpieceOrderLines");
        }
    }
}
