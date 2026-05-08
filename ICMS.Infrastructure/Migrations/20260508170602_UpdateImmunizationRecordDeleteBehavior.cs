using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateImmunizationRecordDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationRecords_VaccinatedIndividuals_IndividualId",
                table: "ImmunizationRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationRecords_VaccinatedIndividuals_IndividualId",
                table: "ImmunizationRecords",
                column: "IndividualId",
                principalTable: "VaccinatedIndividuals",
                principalColumn: "VaccinatedIndividualId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationRecords_VaccinatedIndividuals_IndividualId",
                table: "ImmunizationRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationRecords_VaccinatedIndividuals_IndividualId",
                table: "ImmunizationRecords",
                column: "IndividualId",
                principalTable: "VaccinatedIndividuals",
                principalColumn: "VaccinatedIndividualId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
