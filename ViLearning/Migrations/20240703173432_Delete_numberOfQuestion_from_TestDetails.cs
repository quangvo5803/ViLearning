using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViLearning.Migrations
{
    /// <inheritdoc />
    public partial class Delete_numberOfQuestion_from_TestDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberQuestion",
                table: "TestDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberQuestion",
                table: "TestDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
