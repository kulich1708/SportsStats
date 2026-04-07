using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SportsStats.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRulesTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TournamentsTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TournamentRules_Name = table.Column<string>(type: "text", nullable: false),
                    TournamentRules_PeriodsCount = table.Column<int>(type: "integer", nullable: false),
                    TournamentRules_PeriodDurationSeconds = table.Column<int>(type: "integer", nullable: false),
                    TournamentRules_HasOvertime = table.Column<bool>(type: "boolean", nullable: false),
                    TournamentRules_OvertimeDurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    TournamentRules_OvertimesCount = table.Column<int>(type: "integer", nullable: true),
                    TournamentRules_ShootoutsCount = table.Column<int>(type: "integer", nullable: false),
                    TournamentRules_SuddenDeathOvertime = table.Column<bool>(type: "boolean", nullable: false),
                    TournamentRules_IsDrawPossible = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentsTest", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TournamentsTest");
        }
    }
}
