using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrowFlow_Phoenix.Migrations
{
    /// <inheritdoc />
    public partial class PlacedIsSyncedandLastSyncedpropertiesinExternalIdsandremovedfromEmployeesRemovedLeviathanIdpropasnotneeded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSynced",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LastSyncedAt",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LeviathanId",
                table: "Employees");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSyncedAt",
                table: "EmployeeExternalIds",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSyncedAt",
                table: "EmployeeExternalIds");

            migrationBuilder.AddColumn<bool>(
                name: "IsSynced",
                table: "Employees",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSyncedAt",
                table: "Employees",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LeviathanId",
                table: "Employees",
                type: "TEXT",
                nullable: true);
        }
    }
}
