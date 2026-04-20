using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsStats.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoMime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoMime",
                table: "Tournaments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoMime",
                table: "Teams",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Citizenship_PhotoMime",
                table: "Players",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoMime",
                table: "Players",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoMime",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "PhotoMime",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Citizenship_PhotoMime",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PhotoMime",
                table: "Players");
        }
    }
}
