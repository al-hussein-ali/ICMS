using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedWorkerIdToFieldVisitIndividual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedWorkerId",
                table: "FieldVisitIndividuals",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldVisitIndividuals_AssignedWorkerId",
                table: "FieldVisitIndividuals",
                column: "AssignedWorkerId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldVisitIndividuals_Users_AssignedWorkerId",
                table: "FieldVisitIndividuals",
                column: "AssignedWorkerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldVisitIndividuals_Users_AssignedWorkerId",
                table: "FieldVisitIndividuals");

            migrationBuilder.DropIndex(
                name: "IX_FieldVisitIndividuals_AssignedWorkerId",
                table: "FieldVisitIndividuals");

            migrationBuilder.DropColumn(
                name: "AssignedWorkerId",
                table: "FieldVisitIndividuals");
        }
    }
}
