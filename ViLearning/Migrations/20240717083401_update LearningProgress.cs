using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViLearning.Migrations
{
    /// <inheritdoc />
    public partial class updateLearningProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgresses_AspNetUsers_UserId",
                table: "LearningProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgresses_Courses_CourseId",
                table: "LearningProgresses");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgresses_AspNetUsers_UserId",
                table: "LearningProgresses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgresses_Courses_CourseId",
                table: "LearningProgresses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgresses_AspNetUsers_UserId",
                table: "LearningProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgresses_Courses_CourseId",
                table: "LearningProgresses");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgresses_AspNetUsers_UserId",
                table: "LearningProgresses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgresses_Courses_CourseId",
                table: "LearningProgresses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
