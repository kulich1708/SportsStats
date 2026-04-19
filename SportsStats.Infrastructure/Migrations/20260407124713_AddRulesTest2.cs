using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsStats.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRulesTest2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TournamentRules_HasOvertime",
                table: "TournamentsTest");

            migrationBuilder.DropColumn(
                name: "TournamentRules_IsDrawPossible",
                table: "TournamentsTest");

            migrationBuilder.DropColumn(
                name: "TournamentRules_Name",
                table: "TournamentsTest");

            migrationBuilder.DropColumn(
                name: "TournamentRules_OvertimeDurationSeconds",
                table: "TournamentsTest");

            migrationBuilder.DropColumn(
                name: "TournamentRules_OvertimesCount",
                table: "TournamentsTest");

            migrationBuilder.DropColumn(
                name: "TournamentRules_PeriodDurationSeconds",
                table: "TournamentsTest");

            migrationBuilder.DropColumn(
                name: "TournamentRules_PeriodsCount",
                table: "TournamentsTest");

            migrationBuilder.DropColumn(
                name: "TournamentRules_ShootoutsCount",
                table: "TournamentsTest");

            migrationBuilder.DropColumn(
                name: "TournamentRules_SuddenDeathOvertime",
                table: "TournamentsTest");

            migrationBuilder.CreateTable(
                name: "TournamentMatchRules",
                columns: table => new
                {
                    TournamentTestId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PeriodsCount = table.Column<int>(type: "integer", nullable: false),
                    PeriodDurationSeconds = table.Column<int>(type: "integer", nullable: false),
                    HasOvertime = table.Column<bool>(type: "boolean", nullable: false),
                    OvertimeDurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    OvertimesCount = table.Column<int>(type: "integer", nullable: true),
                    ShootoutsCount = table.Column<int>(type: "integer", nullable: false),
                    SuddenDeathOvertime = table.Column<bool>(type: "boolean", nullable: false),
                    IsDrawPossible = table.Column<bool>(type: "boolean", nullable: false)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TournamentMatchRules");

            migrationBuilder.AddColumn<bool>(
                name: "TournamentRules_HasOvertime",
                table: "TournamentsTest",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TournamentRules_IsDrawPossible",
                table: "TournamentsTest",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TournamentRules_Name",
                table: "TournamentsTest",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TournamentRules_OvertimeDurationSeconds",
                table: "TournamentsTest",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TournamentRules_OvertimesCount",
                table: "TournamentsTest",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TournamentRules_PeriodDurationSeconds",
                table: "TournamentsTest",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TournamentRules_PeriodsCount",
                table: "TournamentsTest",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TournamentRules_ShootoutsCount",
                table: "TournamentsTest",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "TournamentRules_SuddenDeathOvertime",
                table: "TournamentsTest",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
