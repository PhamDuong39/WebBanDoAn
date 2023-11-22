using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebBanDoAn.Migrations
{
    /// <inheritdoc />
    public partial class Update_Db_v1_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3a91aca1-ba34-421a-8fc0-b34e23cc8575");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "59469c9b-b9cd-4201-be0c-95cfff00c0a3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7597706a-88fa-4a73-bcef-22f4a34fc10d");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Discriminator", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "15c42ed0-2928-43e9-9b31-2adea09e4eae", null, "IdentityRole", "Admin", "ADMIN" },
                    { "287925bc-9964-40a8-bdb8-9550575ba37c", null, "IdentityRole", "User", "USER" },
                    { "e624e559-429c-4115-9644-7dd1170d31c3", null, "IdentityRole", "Employee", "EMPLOYEE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "15c42ed0-2928-43e9-9b31-2adea09e4eae");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "287925bc-9964-40a8-bdb8-9550575ba37c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e624e559-429c-4115-9644-7dd1170d31c3");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Discriminator", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3a91aca1-ba34-421a-8fc0-b34e23cc8575", null, "IdentityRole", "Employee", "EMPLOYEE" },
                    { "59469c9b-b9cd-4201-be0c-95cfff00c0a3", null, "IdentityRole", "User", "USER" },
                    { "7597706a-88fa-4a73-bcef-22f4a34fc10d", null, "IdentityRole", "Admin", "ADMIN" }
                });
        }
    }
}
