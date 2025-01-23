using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeestjeOpEenFeestje.Migrations
{
    /// <inheritdoc />
    public partial class finalmanytomanytry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_AspNetUsers_CustomerId",
                table: "Reservations");

            migrationBuilder.DropTable(
                name: "AnimalReservation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Animals",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Reservations",
                newName: "AppUserId1");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_CustomerId",
                table: "Reservations",
                newName: "IX_Reservations_AppUserId1");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AnimalId",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "Reservations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Animals",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Animals",
                table: "Animals",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_AnimalId_Date",
                table: "Reservations",
                columns: new[] { "AnimalId", "Date" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Animals_AnimalId",
                table: "Reservations",
                column: "AnimalId",
                principalTable: "Animals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_AspNetUsers_AppUserId1",
                table: "Reservations",
                column: "AppUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Animals_AnimalId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_AspNetUsers_AppUserId1",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_AnimalId_Date",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Animals",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "AnimalId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Animals");

            migrationBuilder.RenameColumn(
                name: "AppUserId1",
                table: "Reservations",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_AppUserId1",
                table: "Reservations",
                newName: "IX_Reservations_CustomerId");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Animals",
                table: "Animals",
                column: "Name");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_AspNetUsers_CustomerId",
                table: "Reservations",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
