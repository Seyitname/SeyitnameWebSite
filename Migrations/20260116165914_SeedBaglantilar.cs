using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SeyitnameWebSite.Migrations
{
    /// <inheritdoc />
    public partial class SeedBaglantilar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsers",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.InsertData(
                table: "Baglantilar",
                columns: new[] { "Id", "Description", "Link", "Name", "Picture" },
                values: new object[,]
                {
                    { 1, "YouTube kanalımdır ve aktif değildir.", "https://www.youtube.com/@seyitname", "YouTube", "YouTube.png" },
                    { 2, "Satrançta kendimi geliştirmeye çalışıyorum.", "https://www.chess.com/member/seyitnames", "Chesscom", "Chesscom.png" },
                    { 3, "Kulübe istek atan herkesi alabiliriz.", "https://www.chess.com/club/chessname-1/join/757085", "Chesscom Kulüp", "Chesscomkulup.png" },
                    { 4, "Bazen yazdığım kodları paylaşırım.", "https://github.com/seyitname", "GitHub", "GitHub.png" },
                    { 5, "Genelde paylaşım yapmam.", "https://www.instagram.com/seyitname26", "Instagram", "Instagram.jpeg" },
                    { 6, "Yayın açmam ama takip ederseniz sevinirim.", "https://www.kick.com/seyitname", "Kick", "Kick.png" },
                    { 7, "Minecraft karakterim.", "https://laby.net/@seyitname", "Laby", "Laby.png" },
                    { 9, "Bazen burada da satranç oynarım.", "https://lichess.org/@/seyitname", "Lichess", "Lichess.png" },
                    { 10, "Steam profilim.", "https://steamcommunity.com/id/seyitname/", "Steam", "Steam.jpeg" },
                    { 11, "ETS2 oynarım.", "https://truckersmp.com/user/5941855", "TruckersMP", "TruckersMP.png" },
                    { 12, "Yayın açmam ama takip ederseniz sevinirim.", "https://www.twitch.tv/seyitname", "Twitch", "Twitch.png" },
                    { 13, "World of Trucks profilim.", "https://www.worldoftrucks.com/en/profile/10601441", "WorldOfTrucks", "WorldOfTrucks.jpeg" },
                    { 14, "LinkedIn profilim.", "https://www.linkedin.com/in/seyitname", "LinkedIn", "LinkedIn.png" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Baglantilar",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Baglantilar",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Baglantilar",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Baglantilar",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Baglantilar",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Baglantilar",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Baglantilar",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Baglantilar",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Baglantilar",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Baglantilar",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Baglantilar",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Baglantilar",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Baglantilar",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");
        }
    }
}
