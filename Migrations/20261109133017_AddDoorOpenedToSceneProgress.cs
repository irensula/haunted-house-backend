using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddDoorOpenedToSceneProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DoorOpened",
                table: "SceneProgresses",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoorOpened",
                table: "SceneProgresses");
        }
    }
}
