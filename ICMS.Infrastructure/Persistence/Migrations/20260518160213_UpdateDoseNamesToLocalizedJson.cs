using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDoseNamesToLocalizedJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 1,
                column: "DoseName",
                value: "{\"en\":\"BCG\",\"ar\":\"بي سي جي\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 2,
                column: "DoseName",
                value: "{\"en\":\"OPV 0\",\"ar\":\"شلل الأطفال الفموي 0\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 3,
                column: "DoseName",
                value: "{\"en\":\"OPV 1\",\"ar\":\"شلل الأطفال الفموي 1\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 4,
                column: "DoseName",
                value: "{\"en\":\"OPV 2\",\"ar\":\"شلل الأطفال الفموي 2\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 5,
                column: "DoseName",
                value: "{\"en\":\"OPV 3\",\"ar\":\"شلل الأطفال الفموي 3\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 6,
                column: "DoseName",
                value: "{\"en\":\"Penta 1\",\"ar\":\"الخماسي 1\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 7,
                column: "DoseName",
                value: "{\"en\":\"Penta 2\",\"ar\":\"الخماسي 2\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 8,
                column: "DoseName",
                value: "{\"en\":\"Penta 3\",\"ar\":\"الخماسي 3\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 9,
                column: "DoseName",
                value: "{\"en\":\"Rota 1\",\"ar\":\"الروتا 1\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 10,
                column: "DoseName",
                value: "{\"en\":\"Rota 2\",\"ar\":\"الروتا 2\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 11,
                column: "DoseName",
                value: "{\"en\":\"PCV 1\",\"ar\":\"المكورات الرئوية 1\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 12,
                column: "DoseName",
                value: "{\"en\":\"PCV 2\",\"ar\":\"المكورات الرئوية 2\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 13,
                column: "DoseName",
                value: "{\"en\":\"PCV 3\",\"ar\":\"المكورات الرئوية 3\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 14,
                column: "DoseName",
                value: "{\"en\":\"MR 1\",\"ar\":\"الحصبة والحصبة الألمانية 1\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 15,
                column: "DoseName",
                value: "{\"en\":\"MR 2\",\"ar\":\"الحصبة والحصبة الألمانية 2\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 16,
                column: "DoseName",
                value: "{\"en\":\"IPV\",\"ar\":\"شلل الأطفال غير النشط\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 17,
                column: "DoseName",
                value: "{\"en\":\"Vit A 1\",\"ar\":\"فيتامين أ 1\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 18,
                column: "DoseName",
                value: "{\"en\":\"Vit A 2\",\"ar\":\"فيتامين أ 2\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 19,
                column: "DoseName",
                value: "{\"en\":\"TT 1\",\"ar\":\"كزاز 1\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 20,
                column: "DoseName",
                value: "{\"en\":\"TT 2\",\"ar\":\"كزاز 2\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 21,
                column: "DoseName",
                value: "{\"en\":\"TT 3\",\"ar\":\"كزاز 3\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 22,
                column: "DoseName",
                value: "{\"en\":\"TT 4\",\"ar\":\"كزاز 4\"}");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 23,
                column: "DoseName",
                value: "{\"en\":\"TT 5\",\"ar\":\"كزاز 5\"}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 1,
                column: "DoseName",
                value: "BCG");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 2,
                column: "DoseName",
                value: "OPV 0");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 3,
                column: "DoseName",
                value: "OPV 1");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 4,
                column: "DoseName",
                value: "OPV 2");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 5,
                column: "DoseName",
                value: "OPV 3");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 6,
                column: "DoseName",
                value: "Penta 1");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 7,
                column: "DoseName",
                value: "Penta 2");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 8,
                column: "DoseName",
                value: "Penta 3");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 9,
                column: "DoseName",
                value: "Rota 1");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 10,
                column: "DoseName",
                value: "Rota 2");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 11,
                column: "DoseName",
                value: "PCV 1");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 12,
                column: "DoseName",
                value: "PCV 2");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 13,
                column: "DoseName",
                value: "PCV 3");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 14,
                column: "DoseName",
                value: "MR 1");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 15,
                column: "DoseName",
                value: "MR 2");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 16,
                column: "DoseName",
                value: "IPV");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 17,
                column: "DoseName",
                value: "Vit A 1");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 18,
                column: "DoseName",
                value: "Vit A 2");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 19,
                column: "DoseName",
                value: "TT 1");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 20,
                column: "DoseName",
                value: "TT 2");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 21,
                column: "DoseName",
                value: "TT 3");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 22,
                column: "DoseName",
                value: "TT 4");

            migrationBuilder.UpdateData(
                table: "Doses",
                keyColumn: "DoseId",
                keyValue: 23,
                column: "DoseName",
                value: "TT 5");
        }
    }
}
