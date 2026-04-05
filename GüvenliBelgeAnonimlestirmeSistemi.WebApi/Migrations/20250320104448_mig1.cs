using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class mig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Editors",
                columns: table => new
                {
                    EditorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EPosta = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Editors", x => x.EditorId);
                });

            migrationBuilder.CreateTable(
                name: "Fields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlanAdi = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reviewers",
                columns: table => new
                {
                    ReviewerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EPosta = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviewers", x => x.ReviewerId);
                });

            migrationBuilder.CreateTable(
                name: "FieldTopics",
                columns: table => new
                {
                    FieldTopicId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KonuAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FieldId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldTopics", x => x.FieldTopicId);
                    table.ForeignKey(
                        name: "FK_FieldTopics_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    MakaleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YazarEPosta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PdfFilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TakipNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnonimPdfFilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MakaleYuklemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EnSonYapilanIsleminTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MakaleDurumu = table.Column<int>(type: "int", nullable: false),
                    ReviewerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.MakaleId);
                    table.ForeignKey(
                        name: "FK_Articles_Reviewers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Reviewers",
                        principalColumn: "ReviewerId");
                });

            migrationBuilder.CreateTable(
                name: "ReviewerFieldTopics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewerId = table.Column<int>(type: "int", nullable: false),
                    FieldTopicId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewerFieldTopics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewerFieldTopics_FieldTopics_FieldTopicId",
                        column: x => x.FieldTopicId,
                        principalTable: "FieldTopics",
                        principalColumn: "FieldTopicId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewerFieldTopics_Reviewers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Reviewers",
                        principalColumn: "ReviewerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticleFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MakaleId = table.Column<int>(type: "int", nullable: false),
                    FieldTopicId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleFields_Articles_MakaleId",
                        column: x => x.MakaleId,
                        principalTable: "Articles",
                        principalColumn: "MakaleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleFields_FieldTopics_FieldTopicId",
                        column: x => x.FieldTopicId,
                        principalTable: "FieldTopics",
                        principalColumn: "FieldTopicId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogDetayi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    islemZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MakaleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_Logs_Articles_MakaleId",
                        column: x => x.MakaleId,
                        principalTable: "Articles",
                        principalColumn: "MakaleId");
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MakaleId = table.Column<int>(type: "int", nullable: false),
                    SendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SendRol = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_Messages_Articles_MakaleId",
                        column: x => x.MakaleId,
                        principalTable: "Articles",
                        principalColumn: "MakaleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleFields_FieldTopicId",
                table: "ArticleFields",
                column: "FieldTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleFields_MakaleId",
                table: "ArticleFields",
                column: "MakaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ReviewerId",
                table: "Articles",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTopics_FieldId",
                table: "FieldTopics",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_MakaleId",
                table: "Logs",
                column: "MakaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MakaleId",
                table: "Messages",
                column: "MakaleId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewerFieldTopics_FieldTopicId",
                table: "ReviewerFieldTopics",
                column: "FieldTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewerFieldTopics_ReviewerId",
                table: "ReviewerFieldTopics",
                column: "ReviewerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleFields");

            migrationBuilder.DropTable(
                name: "Editors");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "ReviewerFieldTopics");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "FieldTopics");

            migrationBuilder.DropTable(
                name: "Reviewers");

            migrationBuilder.DropTable(
                name: "Fields");
        }
    }
}
