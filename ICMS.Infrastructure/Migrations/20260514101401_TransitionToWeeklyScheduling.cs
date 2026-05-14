using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TransitionToWeeklyScheduling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecommendedAgeInMonths",
                table: "Doses",
                newName: "RecommendedAgeInWeeks");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "Doses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 1,
                column: "IsPrimary",
                value: true);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 2,
                column: "IsPrimary",
                value: true);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 3,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 8 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 4,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 16 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 5,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 24 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 6,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 8 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 7,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 16 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 8,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 24 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 9,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 8 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 10,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 16 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 11,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 8 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 12,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 16 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 13,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 24 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 14,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 36 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 15,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 72 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 16,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 16 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 17,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 24 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 18,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 48 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 19,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 720 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 20,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 724 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 21,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 748 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 22,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 796 });

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 23,
                columns: new[] { "IsPrimary", "RecommendedAgeInWeeks" },
                values: new object[] { true, 844 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "Doses");

            migrationBuilder.RenameColumn(
                name: "RecommendedAgeInWeeks",
                table: "Doses",
                newName: "RecommendedAgeInMonths");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 3,
                column: "RecommendedAgeInMonths",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 4,
                column: "RecommendedAgeInMonths",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 5,
                column: "RecommendedAgeInMonths",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 6,
                column: "RecommendedAgeInMonths",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 7,
                column: "RecommendedAgeInMonths",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 8,
                column: "RecommendedAgeInMonths",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 9,
                column: "RecommendedAgeInMonths",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 10,
                column: "RecommendedAgeInMonths",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 11,
                column: "RecommendedAgeInMonths",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 12,
                column: "RecommendedAgeInMonths",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 13,
                column: "RecommendedAgeInMonths",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 14,
                column: "RecommendedAgeInMonths",
                value: 9);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 15,
                column: "RecommendedAgeInMonths",
                value: 18);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 16,
                column: "RecommendedAgeInMonths",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 17,
                column: "RecommendedAgeInMonths",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 18,
                column: "RecommendedAgeInMonths",
                value: 12);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 19,
                column: "RecommendedAgeInMonths",
                value: 180);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 20,
                column: "RecommendedAgeInMonths",
                value: 181);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 21,
                column: "RecommendedAgeInMonths",
                value: 187);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 22,
                column: "RecommendedAgeInMonths",
                value: 199);

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 23,
                column: "RecommendedAgeInMonths",
                value: 211);
        }
    }
}
