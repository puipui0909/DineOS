using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DineOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToMenuItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SentToKitchenAt",
                table: "order_items",
                type: "datetime(6)",
                nullable: true);
        }
    }
}
