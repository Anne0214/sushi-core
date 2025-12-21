using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiGateway.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fix_DefaultValue_ToSql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "IssuedAt",
                table: "Auth_RefreshToken",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2025, 11, 15, 2, 58, 41, 13, DateTimeKind.Utc).AddTicks(9376));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "IssuedAt",
                table: "Auth_RefreshToken",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 11, 15, 2, 58, 41, 13, DateTimeKind.Utc).AddTicks(9376),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");
        }
    }
}
