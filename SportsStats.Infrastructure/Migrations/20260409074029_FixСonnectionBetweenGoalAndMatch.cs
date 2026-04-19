using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsStats.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixСonnectionBetweenGoalAndMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoalEvent_Matches_MatchId1",
                table: "GoalEvent");

            migrationBuilder.DropIndex(
                name: "IX_GoalEvent_MatchId1",
                table: "GoalEvent");

            migrationBuilder.DropColumn(
                name: "MatchId1",
                table: "GoalEvent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MatchId1",
                table: "GoalEvent",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoalEvent_MatchId1",
                table: "GoalEvent",
                column: "MatchId1");

            migrationBuilder.AddForeignKey(
                name: "FK_GoalEvent_Matches_MatchId1",
                table: "GoalEvent",
                column: "MatchId1",
                principalTable: "Matches",
                principalColumn: "Id");
        }
    }
}
