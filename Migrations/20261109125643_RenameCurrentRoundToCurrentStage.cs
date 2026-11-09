using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameBackend.Migrations
{
    /// <inheritdoc />
    public partial class RenameCurrentRoundToCurrentStage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentRound",
                table: "SceneProgresses",
                newName: "CurrentStage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentStage",
                table: "SceneProgresses",
                newName: "CurrentRound");
        }
    }
}
