using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DineOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSentToKitchen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_sent_to_kitchen",
                table: "order_items",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_sent_to_kitchen",
                table: "order_items");
        }
    }
}
