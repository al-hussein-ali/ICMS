using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixLocationSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 1,
                column: "DirectorateId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 2,
                column: "DirectorateId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 3,
                column: "DirectorateId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 4,
                column: "DirectorateId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 5,
                column: "DirectorateId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 6,
                column: "DirectorateId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 7,
                column: "DirectorateId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 8,
                column: "DirectorateId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 9,
                column: "DirectorateId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 10,
                column: "DirectorateId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 11,
                column: "DirectorateId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 12,
                column: "DirectorateId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 13,
                column: "DirectorateId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 14,
                column: "DirectorateId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 15,
                column: "DirectorateId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 16,
                column: "DirectorateId",
                value: 11);

            migrationBuilder.InsertData(
                table: "Neighborhoods",
                columns: new[] { "Id", "DirectorateId", "Name" },
                values: new object[,]
                {
                    { 17, 1, "السبعين-حي الوحدة" },
                    { 18, 1, "السبعين-حي القادسية" },
                    { 19, 1, "السبعين-حي النصر" }
                });

            migrationBuilder.InsertData(
                table: "SubNeighborhoods",
                columns: new[] { "Id", "Name", "NeighborhoodId" },
                values: new object[,]
                {
                    { 14, "المساكن-المربع الأول", 3 },
                    { 15, "الإنشاءات-المربع التقني", 4 },
                    { 16, "فوة القديمة-حارة الساحل", 5 },
                    { 17, "باعبود-شارع الفنار", 7 },
                    { 18, "المنورة-المربع الغربي", 8 },
                    { 19, "حي أكتوبر-بلوك 5", 9 },
                    { 20, "الغويزي-بجانب القلعة", 11 },
                    { 21, "14 أكتوبر-بلوك 2", 12 },
                    { 22, "جول الشفاء-وحدة الهدى", 13 },
                    { 23, "حي الصيادين-المربع البحري", 16 },
                    { 24, "حي الوحدة-المربع أ", 17 },
                    { 25, "حي القادسية-المربع 1", 18 },
                    { 26, "حي النصر-المربع الشمالي", 19 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_VaccinatedIndividuals_NeighborhoodId",
                table: "VaccinatedIndividuals",
                column: "NeighborhoodId");

            migrationBuilder.AddForeignKey(
                name: "FK_VaccinatedIndividuals_Neighborhoods_NeighborhoodId",
                table: "VaccinatedIndividuals",
                column: "NeighborhoodId",
                principalTable: "Neighborhoods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VaccinatedIndividuals_Neighborhoods_NeighborhoodId",
                table: "VaccinatedIndividuals");

            migrationBuilder.DropIndex(
                name: "IX_VaccinatedIndividuals_NeighborhoodId",
                table: "VaccinatedIndividuals");

            migrationBuilder.DeleteData(
                table: "SubNeighborhoods",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "SubNeighborhoods",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "SubNeighborhoods",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "SubNeighborhoods",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "SubNeighborhoods",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "SubNeighborhoods",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "SubNeighborhoods",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "SubNeighborhoods",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "SubNeighborhoods",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "SubNeighborhoods",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "SubNeighborhoods",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "SubNeighborhoods",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "SubNeighborhoods",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 1,
                column: "DirectorateId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 2,
                column: "DirectorateId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 3,
                column: "DirectorateId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 4,
                column: "DirectorateId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 5,
                column: "DirectorateId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 6,
                column: "DirectorateId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 7,
                column: "DirectorateId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 8,
                column: "DirectorateId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 9,
                column: "DirectorateId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 10,
                column: "DirectorateId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 11,
                column: "DirectorateId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 12,
                column: "DirectorateId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 13,
                column: "DirectorateId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 14,
                column: "DirectorateId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 15,
                column: "DirectorateId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Neighborhoods",
                keyColumn: "Id",
                keyValue: 16,
                column: "DirectorateId",
                value: 1);
        }
    }
}
