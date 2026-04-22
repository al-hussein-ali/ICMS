using Microsoft.EntityFrameworkCore.Migrations;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixAdminPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 999,
                column: "PasswordHash",
                value: "$2a$11$tulMkZYZGUHXu/UNv3EJB.5WfDjvJEaYWjH8UZYHrGAJ/UjBvBlZq");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 999,
                column: "PasswordHash",
                value: "$2a$12$R9h/lIPzHZf5hP1TthHbCuZ8HCTWJ.L7N1S9G0S8T.m6/oM8W9S1.");
        }
    }
}
