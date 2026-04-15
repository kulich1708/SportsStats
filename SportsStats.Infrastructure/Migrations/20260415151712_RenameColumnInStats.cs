using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsStats.Infrastructure.Migrations
{
	/// <inheritdoc />
	public partial class RenameColumnInStats : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropPrimaryKey(
				name: "PK_TeamStats",
				table: "TeamStats");

			migrationBuilder.RenameTable(
				name: "TeamStats",
				newName: "TeamsStats");

			migrationBuilder.RenameColumn(
				name: "RegularWin",
				table: "TeamsStats",
				newName: "RegularWins");

			migrationBuilder.RenameColumn(
				name: "RegularLoss",
				table: "TeamsStats",
				newName: "RegularLosses");

			migrationBuilder.RenameColumn(
				name: "OTWin",
				table: "TeamsStats",
				newName: "OTWins");

			migrationBuilder.RenameColumn(
				name: "OTLoss",
				table: "TeamsStats",
				newName: "OTLosses");

			migrationBuilder.RenameColumn(
				name: "Draw",
				table: "TeamsStats",
				newName: "Draws");

			migrationBuilder.AddColumn<int>(
				name: "Games",
				table: "TeamsStats",
				type: "integer",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddPrimaryKey(
				name: "PK_TeamsStats",
				table: "TeamsStats",
				column: "Id");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropPrimaryKey(
				name: "PK_TeamsStats",
				table: "TeamsStats");

			migrationBuilder.DropColumn(
				name: "Games",
				table: "TeamsStats");

			migrationBuilder.RenameTable(
				name: "TeamsStats",
				newName: "TeamStats");

			migrationBuilder.RenameColumn(
				name: "RegularWins",
				table: "TeamStats",
				newName: "RegularWin");

			migrationBuilder.RenameColumn(
				name: "RegularLosses",
				table: "TeamStats",
				newName: "RegularLoss");

			migrationBuilder.RenameColumn(
				name: "OTWins",
				table: "TeamStats",
				newName: "OTWin");

			migrationBuilder.RenameColumn(
				name: "OTLosses",
				table: "TeamStats",
				newName: "OTLoss");

			migrationBuilder.RenameColumn(
				name: "Draws",
				table: "TeamStats",
				newName: "Draw");

			migrationBuilder.AddPrimaryKey(
				name: "PK_TeamStats",
				table: "TeamStats",
				column: "Id");
		}
	}
}
