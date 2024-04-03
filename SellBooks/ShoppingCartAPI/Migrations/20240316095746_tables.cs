using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingCartAPI.Migrations
{
    /// <inheritdoc />
    public partial class tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cartHeads",
                columns: table => new
                {
                    CartHeadId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cartHeads", x => x.CartHeadId);
                });

            migrationBuilder.CreateTable(
                name: "cartDetail",
                columns: table => new
                {
                    CartDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartHeadId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cartDetail", x => x.CartDetailId);
                    table.ForeignKey(
                        name: "FK_cartDetail_cartHeads_CartHeadId",
                        column: x => x.CartHeadId,
                        principalTable: "cartHeads",
                        principalColumn: "CartHeadId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cartDetail_CartHeadId",
                table: "cartDetail",
                column: "CartHeadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cartDetail");

            migrationBuilder.DropTable(
                name: "cartHeads");

        }
    }
}
