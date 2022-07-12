using Microsoft.EntityFrameworkCore.Migrations;

namespace ToyTrucks.Catalog.Api.Migrations
{
    public partial class removeQuantityandaddOutOfStocktotruck : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "quantity",
                table: "trucks");

            migrationBuilder.AddColumn<bool>(
                name: "out_of_stock",
                table: "trucks",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "out_of_stock",
                table: "trucks");

            migrationBuilder.AddColumn<int>(
                name: "quantity",
                table: "trucks",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }
    }
}
