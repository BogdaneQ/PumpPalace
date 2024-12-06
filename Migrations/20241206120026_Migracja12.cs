using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PumpPalace.Migrations
{
    /// <inheritdoc />
    public partial class Migracja12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NewUntil",
                table: "Products",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewUntil",
                table: "Products");
        }
    }
}
