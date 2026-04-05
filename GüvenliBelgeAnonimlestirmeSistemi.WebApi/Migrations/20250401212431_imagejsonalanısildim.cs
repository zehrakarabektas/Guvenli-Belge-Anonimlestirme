using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class imagejsonalanısildim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptedImagesJson",
                table: "Articles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EncryptedImagesJson",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
