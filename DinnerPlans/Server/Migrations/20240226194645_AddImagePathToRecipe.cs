using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DinnerPlans.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathToRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageFilePath",
                table: "Recipe",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFilePath",
                table: "Recipe");
        }
    }
}
