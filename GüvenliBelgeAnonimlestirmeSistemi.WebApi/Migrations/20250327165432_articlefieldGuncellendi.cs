using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class articlefieldGuncellendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Skor",
                table: "ArticleFields",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Skor",
                table: "ArticleFields");
        }
    }
}
