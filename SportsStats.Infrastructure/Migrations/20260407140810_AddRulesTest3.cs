using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsStats.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRulesTest3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TournamentMatchRules");

            migrationBuilder.AddColumn<string>(
                name: "TournamentRules",
                table: "TournamentsTest",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TournamentRules",
                table: "TournamentsTest");

            migrationBuilder.CreateTable(
                name: "TournamentMatchRules",
                columns: table => new
                {
                    TournamentTestId = table.Column<int>(type: "integer", nullable: false),
                    HasOvertime = table.Column<bool>(type: "boolean", nullable: false),
                    IsDrawPossible = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    OvertimeDurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    OvertimesCount = table.Column<int>(type: "integer", nullable: true),
                    PeriodDurationSeconds = table.Column<int>(type: "integer", nullable: false),
                    PeriodsCount = table.Column<int>(type: "integer", nullable: false),
                    ShootoutsCount = table.Column<int>(type: "integer", nullable: false),
                    SuddenDeathOvertime = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentMatchRules", x => x.TournamentTestId);
                    table.ForeignKey(
                        name: "FK_TournamentMatchRules_TournamentsTest_TournamentTestId",
                        column: x => x.TournamentTestId,
                        principalTable: "TournamentsTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
