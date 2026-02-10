using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_People_UNIQUE_filter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VaccinatedIndividuals_UserId",
                table: "VaccinatedIndividuals");

            migrationBuilder.DropIndex(
                name: "IX_People_PhoneNumber_FirstName_LastName_DateOfBirth",
                table: "People");

            migrationBuilder.CreateIndex(
                name: "IX_VaccinatedIndividuals_UserId",
                table: "VaccinatedIndividuals",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_People_PhoneNumber_FirstName_LastName_DateOfBirth",
                table: "People",
                columns: new[] { "PhoneNumber", "FirstName", "LastName", "DateOfBirth" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VaccinatedIndividuals_UserId",
                table: "VaccinatedIndividuals");

            migrationBuilder.DropIndex(
                name: "IX_People_PhoneNumber_FirstName_LastName_DateOfBirth",
                table: "People");

            migrationBuilder.CreateIndex(
                name: "IX_VaccinatedIndividuals_UserId",
                table: "VaccinatedIndividuals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_People_PhoneNumber_FirstName_LastName_DateOfBirth",
                table: "People",
                columns: new[] { "PhoneNumber", "FirstName", "LastName", "DateOfBirth" },
                unique: true);
        }
    }
}
