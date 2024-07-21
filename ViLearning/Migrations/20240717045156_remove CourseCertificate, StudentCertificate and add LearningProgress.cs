using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViLearning.Migrations
{
    /// <inheritdoc />
    public partial class removeCourseCertificateStudentCertificateandaddLearningProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentCertificates");

            migrationBuilder.DropTable(
                name: "CourseCertificates");

            migrationBuilder.CreateTable(
                name: "LearningProgresses",
                columns: table => new
                {
                    LearningProgressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Progress = table.Column<double>(type: "float", nullable: false),
                    OverallScore = table.Column<double>(type: "float", nullable: false),
                    StudentCertificateUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CourseId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningProgresses", x => x.LearningProgressId);
                    table.ForeignKey(
                        name: "FK_LearningProgresses_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LearningProgresses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LearningProgresses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LearningProgresses_Courses_CourseId1",
                        column: x => x.CourseId1,
                        principalTable: "Courses",
                        principalColumn: "CourseId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LearningProgresses_ApplicationUserId",
                table: "LearningProgresses",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningProgresses_CourseId",
                table: "LearningProgresses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningProgresses_CourseId1",
                table: "LearningProgresses",
                column: "CourseId1");

            migrationBuilder.CreateIndex(
                name: "IX_LearningProgresses_UserId",
                table: "LearningProgresses",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LearningProgresses");

            migrationBuilder.CreateTable(
                name: "CourseCertificates",
                columns: table => new
                {
                    CertificateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    CertificateName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCertificates", x => x.CertificateId);
                    table.ForeignKey(
                        name: "FK_CourseCertificates_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentCertificates",
                columns: table => new
                {
                    CourseCertificateId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCertificates", x => new { x.CourseCertificateId, x.UserId });
                    table.ForeignKey(
                        name: "FK_StudentCertificates_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentCertificates_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentCertificates_CourseCertificates_CourseCertificateId",
                        column: x => x.CourseCertificateId,
                        principalTable: "CourseCertificates",
                        principalColumn: "CertificateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseCertificates_CourseId",
                table: "CourseCertificates",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCertificates_ApplicationUserId",
                table: "StudentCertificates",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCertificates_UserId",
                table: "StudentCertificates",
                column: "UserId");
        }
    }
}
