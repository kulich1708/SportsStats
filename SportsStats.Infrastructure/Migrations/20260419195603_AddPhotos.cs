using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsStats.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "Tournaments",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Teams",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "Teams",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Birthday",
                table: "Players",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CitizenshipName",
                table: "Players",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "CitizenshipPhoto",
                table: "Players",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Players",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "Players",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Birthday",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CitizenshipName",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CitizenshipPhoto",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Players");
        }
    }
}
