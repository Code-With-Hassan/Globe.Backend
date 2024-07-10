using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Globe.Auth.Service.Migrations
{
    /// <inheritdoc />
    public partial class addLastLoggedInPropToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "lastLoggedIn",
                table: "ApplicationUser",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lastLoggedIn",
                table: "ApplicationUser");
        }
    }
}
