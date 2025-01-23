using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeestjeOpEenFeestje.Migrations
{
    /// <inheritdoc />
    public partial class AnimalReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Reservations_ReservationId",
                table: "Animals");

            migrationBuilder.DropIndex(
                name: "IX_Animals_ReservationId",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "ReservationId",
                table: "Animals");

            migrationBuilder.CreateTable(
                name: "AnimalReservation",
                columns: table => new
                {
                    AnimalName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    ReservationDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalReservation", x => new { x.AnimalName, x.ReservationId });
                    table.ForeignKey(
                        name: "FK_AnimalReservation_Animals_AnimalName",
                        column: x => x.AnimalName,
                        principalTable: "Animals",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimalReservation_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnimalReservation_AnimalName_ReservationDate",
                table: "AnimalReservation",
                columns: new[] { "AnimalName", "ReservationDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnimalReservation_ReservationId",
                table: "AnimalReservation",
                column: "ReservationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimalReservation");

            migrationBuilder.AddColumn<int>(
                name: "ReservationId",
                table: "Animals",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Animals_ReservationId",
                table: "Animals",
                column: "ReservationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Reservations_ReservationId",
                table: "Animals",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id");
        }
    }
}
