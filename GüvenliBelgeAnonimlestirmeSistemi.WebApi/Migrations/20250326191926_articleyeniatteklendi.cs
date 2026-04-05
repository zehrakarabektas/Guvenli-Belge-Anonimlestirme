using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class articleyeniatteklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HakemDegerlendirmesi",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SonucPdfFilePath",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HakemDegerlendirmesi",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "SonucPdfFilePath",
                table: "Articles");
        }
    }
}
