using Microsoft.EntityFrameworkCore.Migrations;

namespace MarysToyStore.DataAccess.Migrations
{
    public partial class AddIsArchived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    IsArchived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsArchived = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    EmailAddress = table.Column<string>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: false),
                    IsAdmin = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    ImagePath = table.Column<string>(nullable: true),
                    IsArchived = table.Column<bool>(nullable: false),
                    BrandId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategoryProducts",
                columns: table => new
                {
                    ProductId = table.Column<int>(nullable: false),
                    ProductCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategoryProducts", x => new { x.ProductId, x.ProductCategoryId });
                    table.ForeignKey(
                        name: "FK_ProductCategoryProducts_ProductCategories_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductCategoryProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "IsArchived", "Name" },
                values: new object[] { 1, false, "Mattel" });

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "IsArchived", "Name" },
                values: new object[] { 2, false, "Fisher Price" });

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "IsArchived", "Name" },
                values: new object[] { 3, false, "Hot Wheels" });

            migrationBuilder.InsertData(
                table: "ProductCategories",
                columns: new[] { "Id", "IsArchived", "Name" },
                values: new object[] { 1, false, "Sporting Goods" });

            migrationBuilder.InsertData(
                table: "ProductCategories",
                columns: new[] { "Id", "IsArchived", "Name" },
                values: new object[] { 2, false, "Home" });

            migrationBuilder.InsertData(
                table: "ProductCategories",
                columns: new[] { "Id", "IsArchived", "Name" },
                values: new object[] { 3, false, "Office" });

            migrationBuilder.InsertData(
                table: "ProductCategories",
                columns: new[] { "Id", "IsArchived", "Name" },
                values: new object[] { 4, false, "Clothing" });

            migrationBuilder.InsertData(
                table: "ProductCategories",
                columns: new[] { "Id", "IsArchived", "Name" },
                values: new object[] { 5, false, "Electronics" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "EmailAddress", "FirstName", "IsAdmin", "LastName", "PasswordHash" },
                values: new object[] { 1, "admin@admin.admin", "admin's First name", true, "admin's Last Name", "AQAAAAEAACcQAAAAECRGWYJZrwdhOR60KJ721NpQDZILO8TKmaYAGZEiy2ZKr+8sAKIlfMrd3PHF202xZw==" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BrandId", "Description", "ImagePath", "IsArchived", "Name", "Price" },
                values: new object[] { 2, 1, "Toy ducks that float.", "/ducks.jpg", false, "Ducks", 10.99m });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BrandId", "Description", "ImagePath", "IsArchived", "Name", "Price" },
                values: new object[] { 3, 1, "A toy to build your ideas.", "/legos.jpg", false, "Legos", 25.99m });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BrandId", "Description", "ImagePath", "IsArchived", "Name", "Price" },
                values: new object[] { 4, 1, "An advanced toy that will make anybody happy.", "/robot.jpg", false, "Robot", 15.99m });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BrandId", "Description", "ImagePath", "IsArchived", "Name", "Price" },
                values: new object[] { 5, 2, "A soft bear that is comforting to touch.", "/teddy.jpg", false, "Teddy", 29.99m });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BrandId", "Description", "ImagePath", "IsArchived", "Name", "Price" },
                values: new object[] { 1, 3, "A toy car that goes really fast.", "/bluecar.jpg", false, "Car", 3.99m });

            migrationBuilder.InsertData(
                table: "ProductCategoryProducts",
                columns: new[] { "ProductId", "ProductCategoryId" },
                values: new object[] { 1, 3 });

            migrationBuilder.InsertData(
                table: "ProductCategoryProducts",
                columns: new[] { "ProductId", "ProductCategoryId" },
                values: new object[] { 1, 5 });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategoryProducts_ProductCategoryId",
                table: "ProductCategoryProducts",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductCategoryProducts");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Brands");
        }
    }
}
