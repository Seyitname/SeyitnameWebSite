using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeyitnameWebSite.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectToHomework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Homeworks",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Homeworks");
        }
    }
}
