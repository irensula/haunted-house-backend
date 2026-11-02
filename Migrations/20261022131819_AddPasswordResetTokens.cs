using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "PasswordResetTokens",
                newName: "TokenHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TokenHash",
                table: "PasswordResetTokens",
                newName: "Token");
        }
    }
}
