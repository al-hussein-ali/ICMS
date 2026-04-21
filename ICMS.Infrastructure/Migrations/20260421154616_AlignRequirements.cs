using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlignRequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxEligibleAgeInMonths",
                table: "Vaccines",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinEligibleAgeInMonths",
                table: "Vaccines",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BatchName",
                table: "Batches",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "CreationDate",
                table: "Batches",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxEligibleAgeInMonths",
                table: "Vaccines");

            migrationBuilder.DropColumn(
                name: "MinEligibleAgeInMonths",
                table: "Vaccines");

            migrationBuilder.DropColumn(
                name: "BatchName",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Batches");
        }
    }
}
