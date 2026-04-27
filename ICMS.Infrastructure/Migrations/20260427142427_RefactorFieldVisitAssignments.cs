using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorFieldVisitAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldVisits_Users_AssignedUserId",
                table: "FieldVisits");

            migrationBuilder.DropIndex(
                name: "IX_FieldVisits_AssignedUserId",
                table: "FieldVisits");

            migrationBuilder.DropColumn(
                name: "AssignedUserId",
                table: "FieldVisits");

            migrationBuilder.DropColumn(
                name: "BeneficiaryFromDate",
                table: "FieldVisits");

            migrationBuilder.DropColumn(
                name: "BeneficiaryToDate",
                table: "FieldVisits");

            migrationBuilder.AddColumn<DateOnly>(
                name: "FromDate",
                table: "FieldVisits",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "ToDate",
                table: "FieldVisits",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromDate",
                table: "FieldVisits");

            migrationBuilder.DropColumn(
                name: "ToDate",
                table: "FieldVisits");

            migrationBuilder.AddColumn<int>(
                name: "AssignedUserId",
                table: "FieldVisits",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "BeneficiaryFromDate",
                table: "FieldVisits",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "BeneficiaryToDate",
                table: "FieldVisits",
                type: "date",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldVisits_AssignedUserId",
                table: "FieldVisits",
                column: "AssignedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldVisits_Users_AssignedUserId",
                table: "FieldVisits",
                column: "AssignedUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
