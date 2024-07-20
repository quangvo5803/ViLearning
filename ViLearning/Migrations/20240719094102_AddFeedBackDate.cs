using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViLearning.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedBackDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_AspNetUsers_ApplicationUserId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_AspNetUsers_UserId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Courses_CourseId1",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_ApplicationUserId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_CourseId1",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "CourseId1",
                table: "Feedbacks");

            

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_AspNetUsers_UserId",
                table: "Feedbacks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_AspNetUsers_UserId",
                table: "Feedbacks");

            

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Feedbacks",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourseId1",
                table: "Feedbacks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_ApplicationUserId",
                table: "Feedbacks",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_CourseId1",
                table: "Feedbacks",
                column: "CourseId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_AspNetUsers_ApplicationUserId",
                table: "Feedbacks",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_AspNetUsers_UserId",
                table: "Feedbacks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Courses_CourseId1",
                table: "Feedbacks",
                column: "CourseId1",
                principalTable: "Courses",
                principalColumn: "CourseId");
        }
    }
}
