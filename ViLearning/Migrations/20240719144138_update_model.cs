using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViLearning.Migrations
{
    /// <inheritdoc />
    public partial class update_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LearnedLessons",
                table: "LearningProgresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Feedbacks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LearnedLessons",
                table: "LearningProgresses");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Feedbacks");
        }
    }
}
