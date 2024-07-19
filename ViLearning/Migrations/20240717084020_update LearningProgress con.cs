using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViLearning.Migrations
{
    /// <inheritdoc />
    public partial class updateLearningProgresscon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgresses_AspNetUsers_ApplicationUserId",
                table: "LearningProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgresses_Courses_CourseId1",
                table: "LearningProgresses");

            migrationBuilder.DropIndex(
                name: "IX_LearningProgresses_ApplicationUserId",
                table: "LearningProgresses");

            migrationBuilder.DropIndex(
                name: "IX_LearningProgresses_CourseId1",
                table: "LearningProgresses");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "LearningProgresses");

            migrationBuilder.DropColumn(
                name: "CourseId1",
                table: "LearningProgresses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "LearningProgresses",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourseId1",
                table: "LearningProgresses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LearningProgresses_ApplicationUserId",
                table: "LearningProgresses",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningProgresses_CourseId1",
                table: "LearningProgresses",
                column: "CourseId1");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgresses_AspNetUsers_ApplicationUserId",
                table: "LearningProgresses",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgresses_Courses_CourseId1",
                table: "LearningProgresses",
                column: "CourseId1",
                principalTable: "Courses",
                principalColumn: "CourseId");
        }
    }
}
