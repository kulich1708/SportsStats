using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsStats.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRulesTest5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeamsId",
                table: "Tournaments",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamsId",
                table: "Tournaments");
        }
    }
}
