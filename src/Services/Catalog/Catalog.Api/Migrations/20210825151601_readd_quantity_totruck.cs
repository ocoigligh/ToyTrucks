using Microsoft.EntityFrameworkCore.Migrations;

namespace ToyTrucks.Catalog.Api.Migrations
{
    public partial class readd_quantity_totruck : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "quantity",
                table: "trucks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "quantity",
                table: "trucks");
        }
    }
}
