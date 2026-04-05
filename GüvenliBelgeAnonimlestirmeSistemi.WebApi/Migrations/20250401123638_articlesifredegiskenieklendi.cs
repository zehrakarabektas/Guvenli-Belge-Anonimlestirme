using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class articlesifredegiskenieklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EncryptedInfoJson",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptedInfoJson",
                table: "Articles");
        }
    }
}
