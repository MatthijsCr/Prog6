using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BeestjeOpEenFeestje.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "ImageURL", "Name", "Price", "Type" },
                values: new object[,]
                {
                    { 1, "/images/Monkey.png", "Aap", 100.0, "Jungle" },
                    { 2, "/images/Elephant.png", "Olifant", 200.0, "Jungle" },
                    { 3, "/images/Zebra.png", "Zebra", 150.0, "Jungle" },
                    { 4, "/images/Lion.png", "Leeuw", 300.0, "Jungle" },
                    { 5, "/images/Dog.png", "Hond", 50.0, "Boerderij" },
                    { 6, "/images/Donkey.png", "Ezel", 75.0, "Boerderij" },
                    { 7, "/images/Cow.png", "Koe", 125.0, "Boerderij" },
                    { 8, "/images/Duck.png", "Eend", 30.0, "Boerderij" },
                    { 9, "/images/Chicken.png", "Kuiken", 10.0, "Boerderij" },
                    { 10, "/images/Pinquin.png", "Pinguïn", 80.0, "Sneeuw" },
                    { 11, "/images/PolarBear.png", "IJsbeer", 250.0, "Sneeuw" },
                    { 12, "/images/SeaLion.png", "Zeehond", 100.0, "Sneeuw" },
                    { 13, "/images/Camel.png", "Kameel", 120.0, "Woestijn" },
                    { 14, "/images/Snake.png", "Slang", 90.0, "Woestijn" },
                    { 15, "/images/TRex.png", "T-Rex", 500.0, "VIP" },
                    { 16, "/images/Unicorn.png", "Unicorn", 1000.0, "VIP" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 16);
        }
    }
}
