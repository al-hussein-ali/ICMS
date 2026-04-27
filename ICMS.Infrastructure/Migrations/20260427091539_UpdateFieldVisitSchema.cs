using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFieldVisitSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedUserId",
                table: "FieldVisits",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "BeneficiaryFromDate",
                table: "FieldVisits",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "BeneficiaryToDate",
                table: "FieldVisits",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CampaignName",
                table: "FieldVisits",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

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
                name: "IX_VaccinatedIndividuals_SubNeighborhoodId",
                table: "VaccinatedIndividuals",
                column: "SubNeighborhoodId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldVisits_AssignedUserId",
                table: "FieldVisits",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldVisitTargetedBeneficiary_VaccinatedIndividualId",
                table: "FieldVisitTargetedBeneficiary",
                column: "VaccinatedIndividualId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldVisits_Users_AssignedUserId",
                table: "FieldVisits",
                column: "AssignedUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_VaccinatedIndividuals_SubNeighborhoods_SubNeighborhoodId",
                table: "VaccinatedIndividuals",
                column: "SubNeighborhoodId",
                principalTable: "SubNeighborhoods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldVisits_Users_AssignedUserId",
                table: "FieldVisits");

            migrationBuilder.DropForeignKey(
                name: "FK_VaccinatedIndividuals_SubNeighborhoods_SubNeighborhoodId",
                table: "VaccinatedIndividuals");

            migrationBuilder.DropTable(
                name: "FieldVisitTargetedBeneficiary");

            migrationBuilder.DropIndex(
                name: "IX_VaccinatedIndividuals_SubNeighborhoodId",
                table: "VaccinatedIndividuals");

            migrationBuilder.DropIndex(
                name: "IX_FieldVisits_AssignedUserId",
                table: "FieldVisits");

            migrationBuilder.DropColumn(
                name: "AssignedUserId",
                table: "FieldVisits");

            migrationBuilder.DropColumn(
                name: "BeneficiaryFromDate",
                table: "FieldVisits");

            migrationBuilder.DropColumn(
                name: "BeneficiaryToDate",
                table: "FieldVisits");

            migrationBuilder.DropColumn(
                name: "CampaignName",
                table: "FieldVisits");
        }
    }
}
