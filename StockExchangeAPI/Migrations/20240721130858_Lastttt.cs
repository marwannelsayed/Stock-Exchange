using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockExchangeAPI.Migrations
{
    /// <inheritdoc />
    public partial class Lastttt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subscriber",
                table: "Stocks",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subscriber",
                table: "Stocks");
        }
    }
}
