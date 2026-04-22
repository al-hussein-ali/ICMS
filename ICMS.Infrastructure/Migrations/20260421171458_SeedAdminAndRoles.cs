using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "People",
                columns: new[] { "PersonId", "CreatedAt", "DateOfBirth", "FirstName", "Gender", "LastName", "PhoneNumber", "SecondName", "ThirdName" },
                values: new object[] { 999, new DateTime(2026, 4, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(1990, 1, 1), "System", "???", "User", "777777777", "Admin", null });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { 1001, "Admin" },
                    { 1002, "InventoryManager" },
                    { 1003, "VaccinationManager" },
                    { 1004, "ReproductiveHealthManager" },
                    { 1005, "FieldVisitWorker" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedAt", "IsActive", "PasswordHash", "PersonId", "UserName" },
                values: new object[] { 999, new DateTime(2026, 4, 21, 0, 0, 0, 0, DateTimeKind.Utc), true, "$2a$12$R9h/lIPzHZf5hP1TthHbCuZ8HCTWJ.L7N1S9G0S8T.m6/oM8W9S1.", 999, "admin" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 1001, 999 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1004);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1005);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1001, 999 });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 999);

            migrationBuilder.DeleteData(
                table: "People",
                keyColumn: "PersonId",
                keyValue: 999);
        }
    }
}
