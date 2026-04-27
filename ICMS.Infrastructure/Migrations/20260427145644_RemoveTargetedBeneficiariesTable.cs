using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTargetedBeneficiariesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldVisitTargetedBeneficiary");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FieldVisitTargetedBeneficiary",
                columns: table => new
                {
                    FieldVisitId = table.Column<int>(type: "integer", nullable: false),
                    VaccinatedIndividualId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldVisitTargetedBeneficiary", x => new { x.FieldVisitId, x.VaccinatedIndividualId });
                    table.ForeignKey(
                        name: "FK_FieldVisitTargetedBeneficiary_FieldVisits_FieldVisitId",
                        column: x => x.FieldVisitId,
                        principalTable: "FieldVisits",
                        principalColumn: "FieldVisitId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldVisitTargetedBeneficiary_VaccinatedIndividuals_Vaccina~",
                        column: x => x.VaccinatedIndividualId,
                        principalTable: "VaccinatedIndividuals",
                        principalColumn: "VaccinatedIndividualId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldVisitTargetedBeneficiary_VaccinatedIndividualId",
                table: "FieldVisitTargetedBeneficiary",
                column: "VaccinatedIndividualId");
        }
    }
}
