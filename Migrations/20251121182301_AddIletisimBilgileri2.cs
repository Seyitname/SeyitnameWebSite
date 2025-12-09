using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeyitnameWebSite.Migrations
{
    /// <inheritdoc />
    public partial class AddIletisimBilgileri2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Telefon",
                table: "IBilgiler");

            migrationBuilder.AddColumn<int>(
                name: "Puan",
                table: "IBilgiler",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Puan",
                table: "IBilgiler");

            migrationBuilder.AddColumn<string>(
                name: "Telefon",
                table: "IBilgiler",
                type: "TEXT",
                nullable: true);
        }
    }
}
