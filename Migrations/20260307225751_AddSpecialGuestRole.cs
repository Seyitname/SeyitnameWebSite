using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeyitnameWebSite.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecialGuestRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { "special-guest-role-id", "özel misafir", "ÖZEL MİSAFİR", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyColumnType: "TEXT",
                keyValue: "special-guest-role-id");
        }
    }
}
