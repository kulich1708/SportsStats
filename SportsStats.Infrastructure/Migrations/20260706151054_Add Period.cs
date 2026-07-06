using Microsoft.EntityFrameworkCore.Migrations;
using SportsStats.Domain.Matches;

#nullable disable

namespace SportsStats.Infrastructure.Migrations
{
	/// <inheritdoc />
	public partial class AddPeriod : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<Period>(
				name: "Period",
				table: "Matches",
				type: "jsonb",
				nullable: false,
				defaultValue: "{\"Current\": 0, \"IsBreak\": true, \"Title\": null}");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Period",
				table: "Matches");
		}
	}
}
