using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wordle.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamePointsToPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Points",
                table: "Scores");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Scores",
                newName: "Point");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Point",
                table: "Scores",
                newName: "Value");

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "Scores",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
