using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gathering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditingToCommunityAndSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Sessions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Sessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Communities",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Communities",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Communities");
        }
    }
}
