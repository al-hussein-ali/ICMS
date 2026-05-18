using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVaccineNamesToLocalizedJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 1,
                column: "VaccineName",
                value: "{\"en\":\"BCG\",\"ar\":\"بي سي جي\"}");

            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 2,
                column: "VaccineName",
                value: "{\"en\":\"Oral Polio (OPV)\",\"ar\":\"شلل الأطفال الفموي\"}");

            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 3,
                column: "VaccineName",
                value: "{\"en\":\"Pentavalent\",\"ar\":\"اللقاح الخماسي\"}");

            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 4,
                column: "VaccineName",
                value: "{\"en\":\"Rotavirus\",\"ar\":\"فيروس الروتا\"}");

            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 5,
                column: "VaccineName",
                value: "{\"en\":\"Pneumococcal (PCV)\",\"ar\":\"المكورات الرئوية\"}");

            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 6,
                column: "VaccineName",
                value: "{\"en\":\"Measles & Rubella (MR)\",\"ar\":\"الحصبة والحصبة الألمانية\"}");

            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 7,
                column: "VaccineName",
                value: "{\"en\":\"Inactivated Polio (IPV)\",\"ar\":\"شلل الأطفال غير النشط\"}");

            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 8,
                column: "VaccineName",
                value: "{\"en\":\"Vitamin A\",\"ar\":\"فيتامين أ\"}");

            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 9,
                column: "VaccineName",
                value: "{\"en\":\"Tetanus Toxoid\",\"ar\":\"ذوفان الكزاز\"}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 1,
                column: "VaccineName",
                value: "BCG");

            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 2,
                column: "VaccineName",
                value: "Oral Polio (OPV)");

            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 3,
                column: "VaccineName",
                value: "Pentavalent");

            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 4,
                column: "VaccineName",
                value: "Rotavirus");

            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 5,
                column: "VaccineName",
                value: "Pneumococcal (PCV)");

            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 6,
                column: "VaccineName",
                value: "Measles & Rubella (MR)");

            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 7,
                column: "VaccineName",
                value: "Inactivated Polio (IPV)");

            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 8,
                column: "VaccineName",
                value: "Vitamin A");

            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 9,
                column: "VaccineName",
                value: "Tetanus Toxoid");
        }
    }
}
