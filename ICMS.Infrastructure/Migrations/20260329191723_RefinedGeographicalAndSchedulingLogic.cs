using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefinedGeographicalAndSchedulingLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "GovernorateId", "Name" },
                values: new object[] { 22, "السبعين" });

            migrationBuilder.UpdateData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "GovernorateId", "Name" },
                values: new object[] { 22, "الوحدة" });

            migrationBuilder.UpdateData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "GovernorateId", "Name" },
                values: new object[] { 22, "التحرير" });

            migrationBuilder.UpdateData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "GovernorateId", "Name" },
                values: new object[] { 22, "معين" });

            migrationBuilder.UpdateData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "GovernorateId", "Name" },
                values: new object[] { 22, "الصافية" });

            migrationBuilder.InsertData(
                table: "Directorates",
                columns: new[] { "Id", "GovernorateId", "Name" },
                values: new object[,]
                {
                    { 6, 22, "آزال" },
                    { 7, 22, "صنعاء القديمة" },
                    { 8, 22, "شعوب" },
                    { 9, 22, "الثورة" },
                    { 10, 22, "بني الحارث" },
                    { 11, 5, "مدينة المكلا" },
                    { 12, 5, "أرياف المكلا" },
                    { 13, 5, "الشحر" },
                    { 14, 5, "سيئون" },
                    { 15, 5, "غيل باوزير" }
                });

            migrationBuilder.UpdateData(
                table: "Governorates",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "صنعاء");

            migrationBuilder.UpdateData(
                table: "Governorates",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "تعز");

            migrationBuilder.UpdateData(
                table: "Governorates",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "الحديدة");

            migrationBuilder.UpdateData(
                table: "Governorates",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "حضرموت");

            migrationBuilder.UpdateData(
                table: "Governorates",
                keyColumn: "Id",
                keyValue: 21,
                column: "Name",
                value: "سقطرى");

            migrationBuilder.UpdateData(
                table: "Governorates",
                keyColumn: "Id",
                keyValue: 22,
                column: "Name",
                value: "أمانة العاصمة");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.UpdateData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "GovernorateId", "Name" },
                values: new object[] { 3, "مدينة المكلا" });

            migrationBuilder.UpdateData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "GovernorateId", "Name" },
                values: new object[] { 3, "أرياف المكلا" });

            migrationBuilder.UpdateData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "GovernorateId", "Name" },
                values: new object[] { 3, "الشحر" });

            migrationBuilder.UpdateData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "GovernorateId", "Name" },
                values: new object[] { 3, "سيئون" });

            migrationBuilder.UpdateData(
                table: "Directorates",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "GovernorateId", "Name" },
                values: new object[] { 3, "غيل باوزير" });

            migrationBuilder.UpdateData(
                table: "Governorates",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "أمانة العاصمة");

            migrationBuilder.UpdateData(
                table: "Governorates",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "حضرموت");

            migrationBuilder.UpdateData(
                table: "Governorates",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "تعز");

            migrationBuilder.UpdateData(
                table: "Governorates",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "الحديدة");

            migrationBuilder.UpdateData(
                table: "Governorates",
                keyColumn: "Id",
                keyValue: 21,
                column: "Name",
                value: "صنعاء");

            migrationBuilder.UpdateData(
                table: "Governorates",
                keyColumn: "Id",
                keyValue: 22,
                column: "Name",
                value: "سقطرى");
        }
    }
}
