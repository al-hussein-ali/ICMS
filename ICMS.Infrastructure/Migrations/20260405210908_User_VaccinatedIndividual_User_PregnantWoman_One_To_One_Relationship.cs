using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class User_VaccinatedIndividual_User_PregnantWoman_One_To_One_Relationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PregnantWomen_People_PersonId",
                table: "PregnantWomen");

            migrationBuilder.DropForeignKey(
                name: "FK_PregnantWomen_Users_UserId",
                table: "PregnantWomen");

            migrationBuilder.DropForeignKey(
                name: "FK_VaccinatedIndividuals_Users_UserId",
                table: "VaccinatedIndividuals");

            migrationBuilder.DropIndex(
                name: "IX_PregnantWomen_UserId",
                table: "PregnantWomen");

            migrationBuilder.RenameIndex(
                name: "IX_VaccinatedIndividuals_People_Users1",
                table: "PregnantWomen",
                newName: "IX_PregnantWomen_People_Users");

            migrationBuilder.CreateIndex(
                name: "IX_PregnantWomen_PersonId",
                table: "PregnantWomen",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PregnantWomen_UserId",
                table: "PregnantWomen",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PregnantWomen_People_PersonId",
                table: "PregnantWomen",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PregnantWomen_Users_UserId",
                table: "PregnantWomen",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_VaccinatedIndividuals_Users_UserId",
                table: "VaccinatedIndividuals",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PregnantWomen_People_PersonId",
                table: "PregnantWomen");

            migrationBuilder.DropForeignKey(
                name: "FK_PregnantWomen_Users_UserId",
                table: "PregnantWomen");

            migrationBuilder.DropForeignKey(
                name: "FK_VaccinatedIndividuals_Users_UserId",
                table: "VaccinatedIndividuals");

            migrationBuilder.DropIndex(
                name: "IX_PregnantWomen_PersonId",
                table: "PregnantWomen");

            migrationBuilder.DropIndex(
                name: "IX_PregnantWomen_UserId",
                table: "PregnantWomen");

            migrationBuilder.RenameIndex(
                name: "IX_PregnantWomen_People_Users",
                table: "PregnantWomen",
                newName: "IX_VaccinatedIndividuals_People_Users1");

            migrationBuilder.CreateIndex(
                name: "IX_PregnantWomen_UserId",
                table: "PregnantWomen",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PregnantWomen_People_PersonId",
                table: "PregnantWomen",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PregnantWomen_Users_UserId",
                table: "PregnantWomen",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_VaccinatedIndividuals_Users_UserId",
                table: "VaccinatedIndividuals",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
