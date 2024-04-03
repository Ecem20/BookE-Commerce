using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookAPI.Migrations
{
    /// <inheritdoc />
    public partial class deletetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "BookId",
                keyValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "books",
                columns: new[] { "BookId", "Author", "Category", "ISBNNo", "Price", "Publisher", "StockQuantity", "Title" },
                values: new object[] { 1, "Joseph Murphy", "Psikoloji", "12345678", 100m, "Can", 5, "Bilinçaltının Gücü" });
        }
    }
}
