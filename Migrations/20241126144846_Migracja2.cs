using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PumpPalace.Migrations
{
    /// <inheritdoc />
    public partial class Migracja2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "Customers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetTokenExpiry",
                table: "Customers",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PasswordResetTokenExpiry",
                table: "Customers");
        }
    }
}
