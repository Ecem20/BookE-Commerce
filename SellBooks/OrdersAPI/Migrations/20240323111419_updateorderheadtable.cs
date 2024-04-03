using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdersAPI.Migrations
{
    /// <inheritdoc />
    public partial class updateorderheadtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "orderHeads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "orderHeads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SurName",
                table: "orderHeads",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "orderHeads");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "orderHeads");

            migrationBuilder.DropColumn(
                name: "SurName",
                table: "orderHeads");
        }
    }
}
