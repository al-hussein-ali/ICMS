using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTetanusAndCsvReporting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 19,
                columns: new[] { "RecommendedAgeGroup", "RecommendedAgeInMonths" },
                values: new object[] { "female15_49", 180 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 20,
                columns: new[] { "RecommendedAgeGroup", "RecommendedAgeInMonths" },
                values: new object[] { "female15_49", 181 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 21,
                columns: new[] { "RecommendedAgeGroup", "RecommendedAgeInMonths" },
                values: new object[] { "female15_49", 187 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 22,
                columns: new[] { "RecommendedAgeGroup", "RecommendedAgeInMonths" },
                values: new object[] { "female15_49", 199 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 23,
                columns: new[] { "RecommendedAgeGroup", "RecommendedAgeInMonths" },
                values: new object[] { "female15_49", 211 });

            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 9,
                column: "Audience",
                value: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 19,
                columns: new[] { "RecommendedAgeGroup", "RecommendedAgeInMonths" },
                values: new object[] { "pregnancy", 0 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 20,
                columns: new[] { "RecommendedAgeGroup", "RecommendedAgeInMonths" },
                values: new object[] { "pregnancy", 1 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 21,
                columns: new[] { "RecommendedAgeGroup", "RecommendedAgeInMonths" },
                values: new object[] { "pregnancy", 6 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 22,
                columns: new[] { "RecommendedAgeGroup", "RecommendedAgeInMonths" },
                values: new object[] { "pregnancy", 12 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 23,
                columns: new[] { "RecommendedAgeGroup", "RecommendedAgeInMonths" },
                values: new object[] { "pregnancy", 24 });

            migrationBuilder.UpdateData(
                table: "Vaccines",
                keyColumn: "VaccineId",
                keyValue: 9,
                column: "Audience",
                value: 2);
        }
    }
}
