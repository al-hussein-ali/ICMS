using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGeographicalNormalizationAndSchedules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area",
                table: "VaccinatedIndividuals");

            migrationBuilder.DropColumn(
                name: "Directorate",
                table: "VaccinatedIndividuals");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "VaccinatedIndividuals");

            migrationBuilder.RenameIndex(
                name: "IX_VaccinatedIndividuals_People_Users",
                table: "PregnantWomen",
                newName: "IX_VaccinatedIndividuals_People_Users_TEMP");

            migrationBuilder.RenameIndex(
                name: "IX_VaccinatedIndividuals_People_Users1",
                table: "VaccinatedIndividuals",
                newName: "IX_VaccinatedIndividuals_People_Users");

            migrationBuilder.RenameIndex(
                name: "IX_VaccinatedIndividuals_People_Users_TEMP",
                table: "PregnantWomen",
                newName: "IX_VaccinatedIndividuals_People_Users1");

            migrationBuilder.AddColumn<int>(
                name: "DirectorateId",
                table: "VaccinatedIndividuals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NeighborhoodId",
                table: "VaccinatedIndividuals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "OfflineSyncId",
                table: "VaccinatedIndividuals",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubNeighborhoodId",
                table: "VaccinatedIndividuals",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Governorates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Governorates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VaccinationSchedules",
                columns: table => new
                {
                    VaccinationScheduleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VaccinatedIndividualId = table.Column<int>(type: "integer", nullable: false),
                    DoseId = table.Column<int>(type: "integer", nullable: false),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ActualDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ImmunizationRecordId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaccinationSchedules", x => x.VaccinationScheduleId);
                    table.ForeignKey(
                        name: "FK_VaccinationSchedules_Doses_DoseId",
                        column: x => x.DoseId,
                        principalTable: "Doses",
                        principalColumn: "DoseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VaccinationSchedules_ImmunizationRecords_ImmunizationRecord~",
                        column: x => x.ImmunizationRecordId,
                        principalTable: "ImmunizationRecords",
                        principalColumn: "ImmunizationRecordId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VaccinationSchedules_VaccinatedIndividuals_VaccinatedIndivi~",
                        column: x => x.VaccinatedIndividualId,
                        principalTable: "VaccinatedIndividuals",
                        principalColumn: "VaccinatedIndividualId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Directorates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    GovernorateId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directorates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Directorates_Governorates_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governorates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Neighborhoods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DirectorateId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Neighborhoods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Neighborhoods_Directorates_DirectorateId",
                        column: x => x.DirectorateId,
                        principalTable: "Directorates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubNeighborhoods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NeighborhoodId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubNeighborhoods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubNeighborhoods_Neighborhoods_NeighborhoodId",
                        column: x => x.NeighborhoodId,
                        principalTable: "Neighborhoods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Governorates",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "أمانة العاصمة" },
                    { 2, "عدن" },
                    { 3, "حضرموت" },
                    { 4, "تعز" },
                    { 5, "الحديدة" },
                    { 6, "إب" },
                    { 7, "أبين" },
                    { 8, "البيضاء" },
                    { 9, "لحج" },
                    { 10, "مأرب" },
                    { 11, "شبوة" },
                    { 12, "الجوف" },
                    { 13, "المهرة" },
                    { 14, "المحويت" },
                    { 15, "صعدة" },
                    { 16, "حجة" },
                    { 17, "الضالع" },
                    { 18, "عمران" },
                    { 19, "ذمار" },
                    { 20, "ريمة" },
                    { 21, "صنعاء" },
                    { 22, "سقطرى" }
                });

            migrationBuilder.InsertData(
                table: "Directorates",
                columns: new[] { "Id", "GovernorateId", "Name" },
                values: new object[,]
                {
                    { 1, 3, "مدينة المكلا" },
                    { 2, 3, "أرياف المكلا" },
                    { 3, 3, "الشحر" },
                    { 4, 3, "سيئون" },
                    { 5, 3, "غيل باوزير" }
                });

            migrationBuilder.InsertData(
                table: "Neighborhoods",
                columns: new[] { "Id", "DirectorateId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "فوة-المتضررين" },
                    { 2, 1, "فوة-ابن سيناء" },
                    { 3, 1, "فوة-المساكن" },
                    { 4, 1, "فوة-الإنشاءات" },
                    { 5, 1, "فوة-القديمة" },
                    { 6, 1, "الشرج-حي العمال" },
                    { 7, 1, "الشرج-باعبود" },
                    { 8, 1, "الشرج-المنورة" },
                    { 9, 1, "الشرج-حي أكتوبر" },
                    { 10, 1, "الديس-شعب البادية" },
                    { 11, 1, "الديس-الغويزي" },
                    { 12, 1, "الديس-حي 14 أكتوبر" },
                    { 13, 1, "الديس-جول الشفاء" },
                    { 14, 1, "المكلا-حي الشهيد" },
                    { 15, 1, "المكلا-حي السلام" },
                    { 16, 1, "المكلا-حي الصيادين" }
                });

            migrationBuilder.InsertData(
                table: "SubNeighborhoods",
                columns: new[] { "Id", "Name", "NeighborhoodId" },
                values: new object[,]
                {
                    { 1, "المتضررين-وحدة الصديق", 1 },
                    { 2, "المتضررين-وحدة الربوة", 1 },
                    { 3, "المتضررين-المنطقة المرتفعة", 1 },
                    { 4, "ابن سيناء-مربع السكن الجامعي", 2 },
                    { 5, "ابن سيناء-حي الكوادر", 2 },
                    { 6, "ابن سيناء-منطقة المستشفى", 2 },
                    { 7, "حي العمال-مربع الورش", 6 },
                    { 8, "حي العمال-عمارة باجرش", 6 },
                    { 9, "شعب البادية-حارة الروضة", 10 },
                    { 10, "شعب البادية-جول الغليلة", 10 },
                    { 11, "حي الشهيد-سكة يعقوب", 14 },
                    { 12, "حي الشهيد-حارة البلاد", 14 },
                    { 13, "حي السلام-خلف البريد", 15 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Directorates_GovernorateId",
                table: "Directorates",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_Neighborhoods_DirectorateId",
                table: "Neighborhoods",
                column: "DirectorateId");

            migrationBuilder.CreateIndex(
                name: "IX_SubNeighborhoods_NeighborhoodId",
                table: "SubNeighborhoods",
                column: "NeighborhoodId");

            migrationBuilder.CreateIndex(
                name: "IX_VaccinationSchedules_DoseId",
                table: "VaccinationSchedules",
                column: "DoseId");

            migrationBuilder.CreateIndex(
                name: "IX_VaccinationSchedules_ImmunizationRecordId",
                table: "VaccinationSchedules",
                column: "ImmunizationRecordId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VaccinationSchedules_VaccinatedIndividualId",
                table: "VaccinationSchedules",
                column: "VaccinatedIndividualId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubNeighborhoods");

            migrationBuilder.DropTable(
                name: "VaccinationSchedules");

            migrationBuilder.DropTable(
                name: "Neighborhoods");

            migrationBuilder.DropTable(
                name: "Directorates");

            migrationBuilder.DropTable(
                name: "Governorates");

            migrationBuilder.DropColumn(
                name: "DirectorateId",
                table: "VaccinatedIndividuals");

            migrationBuilder.DropColumn(
                name: "NeighborhoodId",
                table: "VaccinatedIndividuals");

            migrationBuilder.DropColumn(
                name: "OfflineSyncId",
                table: "VaccinatedIndividuals");

            migrationBuilder.DropColumn(
                name: "SubNeighborhoodId",
                table: "VaccinatedIndividuals");

            migrationBuilder.RenameIndex(
                name: "IX_VaccinatedIndividuals_People_Users1",
                table: "PregnantWomen",
                newName: "IX_VaccinatedIndividuals_People_Users_TEMP");

            migrationBuilder.RenameIndex(
                name: "IX_VaccinatedIndividuals_People_Users",
                table: "VaccinatedIndividuals",
                newName: "IX_VaccinatedIndividuals_People_Users1");

            migrationBuilder.RenameIndex(
                name: "IX_VaccinatedIndividuals_People_Users_TEMP",
                table: "PregnantWomen",
                newName: "IX_VaccinatedIndividuals_People_Users");

            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "VaccinatedIndividuals",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Directorate",
                table: "VaccinatedIndividuals",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "VaccinatedIndividuals",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
