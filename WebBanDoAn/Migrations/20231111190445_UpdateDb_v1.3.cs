using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebBanDoAn.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb_v13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "368e4960-0cbd-48e3-90de-d12bca12a800");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "39c21464-55f1-4c90-81e2-13775dbc2df6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d507665f-d8c7-4c6e-809d-e99a95aa2030");

            migrationBuilder.RenameColumn(
                name: "ResetPasswordToken",
                table: "AspNetUsers",
                newName: "RefreshToken");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Discriminator", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "164561a1-6226-4ea5-88e9-7dc683fcc7f8", null, "IdentityRole", "Employee", "EMPLOYEE" },
                    { "e24bfd94-94ac-4458-b7ea-3e6e5398472c", null, "IdentityRole", "User", "USER" },
                    { "eaa5c8e5-1b5b-469a-9c50-fc18080fa353", null, "IdentityRole", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "164561a1-6226-4ea5-88e9-7dc683fcc7f8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e24bfd94-94ac-4458-b7ea-3e6e5398472c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "eaa5c8e5-1b5b-469a-9c50-fc18080fa353");

            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "AspNetUsers",
                newName: "ResetPasswordToken");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Discriminator", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "368e4960-0cbd-48e3-90de-d12bca12a800", null, "IdentityRole", "Admin", "ADMIN" },
                    { "39c21464-55f1-4c90-81e2-13775dbc2df6", null, "IdentityRole", "User", "USER" },
                    { "d507665f-d8c7-4c6e-809d-e99a95aa2030", null, "IdentityRole", "Employee", "EMPLOYEE" }
                });
        }
    }
}
