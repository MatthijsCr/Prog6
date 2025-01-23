using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeestjeOpEenFeestje.Migrations
{
    /// <inheritdoc />
    public partial class manyToManytry3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AnimalReservation_AnimalName_ReservationDate",
                table: "AnimalReservation",
                columns: new[] { "AnimalName", "ReservationDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AnimalReservation_AnimalName_ReservationDate",
                table: "AnimalReservation");
        }
    }
}
