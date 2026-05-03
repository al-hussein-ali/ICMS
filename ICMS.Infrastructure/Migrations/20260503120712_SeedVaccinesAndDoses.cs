using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedVaccinesAndDoses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Vaccines",
                columns: new[] { "VaccineId", "Audience", "Description", "IsActive", "MaxEligibleAgeInMonths", "MinEligibleAgeInMonths", "TotalDosages", "VaccineCode", "VaccineName" },
                values: new object[,]
                {
                    { 1, 1, null, true, 1, 0, (byte)1, "BCG", "BCG" },
                    { 2, 1, null, true, 60, 0, (byte)4, "OPV", "Oral Polio (OPV)" },
                    { 3, 1, null, true, 12, 1, (byte)3, "PENTA", "Pentavalent" },
                    { 4, 1, null, true, 8, 1, (byte)2, "ROTA", "Rotavirus" },
                    { 5, 1, null, true, 24, 1, (byte)3, "PCV", "Pneumococcal (PCV)" },
                    { 6, 1, null, true, 60, 9, (byte)2, "MR", "Measles & Rubella (MR)" },
                    { 7, 1, null, true, 12, 3, (byte)1, "IPV", "Inactivated Polio (IPV)" },
                    { 8, 1, null, true, 60, 6, (byte)2, "VITA", "Vitamin A" },
                    { 9, 2, null, true, 600, 180, (byte)5, "TT", "Tetanus Toxoid" }
                });

            migrationBuilder.InsertData(
                table: "Doses",
                columns: new[] { "DoseId", "DoseName", "DoseOrder", "Notes", "RecommendedAgeGroup", "RecommendedAgeInMonths", "VaccineId" },
                values: new object[,]
                {
                    { 1, "BCG", (byte)1, null, "atBirth", 0, 1 },
                    { 2, "OPV 0", (byte)1, null, "atBirth", 0, 2 },
                    { 3, "OPV 1", (byte)2, null, "2months", 2, 2 },
                    { 4, "OPV 2", (byte)3, null, "4months", 4, 2 },
                    { 5, "OPV 3", (byte)4, null, "6months", 6, 2 },
                    { 6, "Penta 1", (byte)1, null, "2months", 2, 3 },
                    { 7, "Penta 2", (byte)2, null, "4months", 4, 3 },
                    { 8, "Penta 3", (byte)3, null, "6months", 6, 3 },
                    { 9, "Rota 1", (byte)1, null, "2months", 2, 4 },
                    { 10, "Rota 2", (byte)2, null, "4months", 4, 4 },
                    { 11, "PCV 1", (byte)1, null, "2months", 2, 5 },
                    { 12, "PCV 2", (byte)2, null, "4months", 4, 5 },
                    { 13, "PCV 3", (byte)3, null, "6months", 6, 5 },
                    { 14, "MR 1", (byte)1, null, "9months", 9, 6 },
                    { 15, "MR 2", (byte)2, null, "18months", 18, 6 },
                    { 16, "IPV", (byte)1, null, "4months", 4, 7 },
                    { 17, "Vit A 1", (byte)1, null, "6months", 6, 8 },
                    { 18, "Vit A 2", (byte)2, null, "12months", 12, 8 },
                    { 19, "TT 1", (byte)1, null, "pregnancy", 0, 9 },
                    { 20, "TT 2", (byte)2, null, "pregnancy", 1, 9 },
                    { 21, "TT 3", (byte)3, null, "pregnancy", 6, 9 },
                    { 22, "TT 4", (byte)4, null, "pregnancy", 12, 9 },
                    { 23, "TT 5", (byte)5, null, "pregnancy", 24, 9 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 9);
        }
    }
}
