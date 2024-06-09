using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ViLearning.Migrations
{
    /// <inheritdoc />
    public partial class AddFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "CourseId", "CourseName", "CoverImgUrl", "Description", "Price", "SubjectId", "UserId" },
                values: new object[,]
                {
                    { 1, "Toán lớp 1", "aaa.png", null, null, 1, "9999c715-539e-4c36-842d-712c6a5ec32e" },
                    { 2, "Toán lớp 2", "bbb.png", null, null, 1, "9999c715-539e-4c36-842d-712c6a5ec32e" }
                });

            migrationBuilder.InsertData(
                table: "Lessons",
                columns: new[] { "LessonId", "CourseId", "LessonName", "NumberOfQuestion", "statusBoolean" },
                values: new object[,]
                {
                    { 1, 1, "Bài 1: Cộng, trừ", 3, true },
                    { 2, 1, "Bài 2: Nhân, chia", 3, true },
                    { 3, 2, "Bài 1: Chu vi", 3, true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "LessonId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "LessonId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "LessonId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 2);
        }
    }
}
