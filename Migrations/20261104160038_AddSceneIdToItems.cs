using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddSceneIdToItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SceneId",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE \"Items\" SET \"SceneId\" = 1;");

            migrationBuilder.AlterColumn<int>(
                name: "SceneId",
                table: "Items",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_SceneId",
                table: "Items",
                column: "SceneId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Scenes_SceneId",
                table: "Items",
                column: "SceneId",
                principalTable: "Scenes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Scenes_SceneId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_SceneId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SceneId",
                table: "Items");
        }
    }
}
