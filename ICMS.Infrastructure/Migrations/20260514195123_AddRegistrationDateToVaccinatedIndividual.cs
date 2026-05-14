using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrationDateToVaccinatedIndividual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "RegistrationDate",
                table: "VaccinatedIndividuals",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            // Backfill RegistrationDate from Person.CreatedAt for existing records
            migrationBuilder.Sql(
                "UPDATE \"VaccinatedIndividuals\" SET \"RegistrationDate\" = p.\"CreatedAt\"::date FROM \"People\" p WHERE \"VaccinatedIndividuals\".\"PersonId\" = p.\"PersonId\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "VaccinatedIndividuals");
        }
    }
}
