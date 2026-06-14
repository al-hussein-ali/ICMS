using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldVisitIndividuals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FieldVisitIndividuals",
                columns: table => new
                {
                    FieldVisitId = table.Column<int>(type: "integer", nullable: false),
                    VaccinatedIndividualId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldVisitIndividuals", x => new { x.FieldVisitId, x.VaccinatedIndividualId });
                    table.ForeignKey(
                        name: "FK_FieldVisitIndividuals_FieldVisits_FieldVisitId",
                        column: x => x.FieldVisitId,
                        principalTable: "FieldVisits",
                        principalColumn: "FieldVisitId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldVisitIndividuals_VaccinatedIndividuals_VaccinatedIndiv~",
                        column: x => x.VaccinatedIndividualId,
                        principalTable: "VaccinatedIndividuals",
                        principalColumn: "VaccinatedIndividualId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldVisitIndividuals_VaccinatedIndividualId",
                table: "FieldVisitIndividuals",
                column: "VaccinatedIndividualId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldVisitIndividuals");
        }
    }
}
