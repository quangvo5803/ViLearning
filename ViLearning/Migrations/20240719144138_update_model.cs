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

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LearnedLessons",
                table: "LearningProgresses");

            
        }
    }
}
