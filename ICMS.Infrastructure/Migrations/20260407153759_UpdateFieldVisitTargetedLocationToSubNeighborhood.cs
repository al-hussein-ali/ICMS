using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFieldVisitTargetedLocationToSubNeighborhood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetedLocation",
                table: "FieldVisits");

            migrationBuilder.AddColumn<int>(
                name: "SubNeighborhoodId",
                table: "FieldVisits",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VaccinatedIndividuals_DirectorateId",
                table: "VaccinatedIndividuals",
                column: "DirectorateId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldVisits_SubNeighborhoodId",
                table: "FieldVisits",
                column: "SubNeighborhoodId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldVisits_SubNeighborhoods_SubNeighborhoodId",
                table: "FieldVisits",
                column: "SubNeighborhoodId",
                principalTable: "SubNeighborhoods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
                UPDATE ""VaccinatedIndividuals""
                SET ""DirectorateId"" = (SELECT ""Id"" FROM ""Directorates"" LIMIT 1)
                WHERE ""DirectorateId"" NOT IN (SELECT ""Id"" FROM ""Directorates"");
            ");

            migrationBuilder.Sql(@"
                UPDATE ""FieldVisits""
                SET ""SubNeighborhoodId"" = (SELECT ""Id"" FROM ""SubNeighborhoods"" LIMIT 1)
                WHERE ""SubNeighborhoodId"" NOT IN (SELECT ""Id"" FROM ""SubNeighborhoods"");
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_VaccinatedIndividuals_Directorates_DirectorateId",
                table: "VaccinatedIndividuals",
                column: "DirectorateId",
                principalTable: "Directorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldVisits_SubNeighborhoods_SubNeighborhoodId",
                table: "FieldVisits");

            migrationBuilder.DropForeignKey(
                name: "FK_VaccinatedIndividuals_Directorates_DirectorateId",
                table: "VaccinatedIndividuals");

            migrationBuilder.DropIndex(
                name: "IX_VaccinatedIndividuals_DirectorateId",
                table: "VaccinatedIndividuals");

            migrationBuilder.DropIndex(
                name: "IX_FieldVisits_SubNeighborhoodId",
                table: "FieldVisits");

            migrationBuilder.DropColumn(
                name: "SubNeighborhoodId",
                table: "FieldVisits");

            migrationBuilder.AddColumn<string>(
                name: "TargetedLocation",
                table: "FieldVisits",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }
    }
}
