using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CanteenAutomation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class @new : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "90F36B5C-4E3D-5B9F-A2E3-1C9G2H3I4J5K");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "A0G47C6D-5F4E-6C0G-B3F4-2D0H3I4J5K6L");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "80F25A4B-3D2C-4A8E-9B2D-0B8F9A1C2D3E", "B0H58D7E-6G5F-7D1H-C4G5-3E1I4J5K6L7M" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "80F25A4B-3D2C-4A8E-9B2D-0B8F9A1C2D3E");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "B0H58D7E-6G5F-7D1H-C4G5-3E1I4J5K6L7M");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "80F25A4B-3D2C-4A8E-9B2D-0B8F9A1C2D3E", null, "Admin", "ADMIN" },
                    { "90F36B5C-4E3D-5B9F-A2E3-1C9G2H3I4J5K", null, "Staff", "STAFF" },
                    { "A0G47C6D-5F4E-6C0G-B3F4-2D0H3I4J5K6L", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Role", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "B0H58D7E-6G5F-7D1H-C4G5-3E1I4J5K6L7M", 0, "e4aca434-e455-41e6-ae2f-f0ba7fc8a4ef", "admin@canteen.com", true, false, null, "ADMIN@CANTEEN.COM", "ADMIN", "AQAAAAIAAYagAAAAEAFy8CHw83SsSJwc3fNaW8i7gmN9TrcBn5q+Cmrz5u1EVNHrDyZmpc0t+Vo4CmXBoA==", null, false, "Admin", "d857686e-82ba-492f-8b92-361cfd4dbecb", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "80F25A4B-3D2C-4A8E-9B2D-0B8F9A1C2D3E", "B0H58D7E-6G5F-7D1H-C4G5-3E1I4J5K6L7M" });
        }
    }
}
