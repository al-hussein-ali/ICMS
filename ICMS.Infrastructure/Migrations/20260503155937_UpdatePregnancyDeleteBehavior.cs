using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePregnancyDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Newborns_PregnancyDetails_PregnancyDetailsId",
                table: "Newborns");

            migrationBuilder.DropForeignKey(
                name: "FK_PregnancyDetails_PregnantWomen_PregnantWomanId",
                table: "PregnancyDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_Newborns_PregnancyDetails_PregnancyDetailsId",
                table: "Newborns",
                column: "PregnancyDetailsId",
                principalTable: "PregnancyDetails",
                principalColumn: "PregnancyDetailsId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PregnancyDetails_PregnantWomen_PregnantWomanId",
                table: "PregnancyDetails",
                column: "PregnantWomanId",
                principalTable: "PregnantWomen",
                principalColumn: "PregnantWomanId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Newborns_PregnancyDetails_PregnancyDetailsId",
                table: "Newborns");

            migrationBuilder.DropForeignKey(
                name: "FK_PregnancyDetails_PregnantWomen_PregnantWomanId",
                table: "PregnancyDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_Newborns_PregnancyDetails_PregnancyDetailsId",
                table: "Newborns",
                column: "PregnancyDetailsId",
                principalTable: "PregnancyDetails",
                principalColumn: "PregnancyDetailsId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PregnancyDetails_PregnantWomen_PregnantWomanId",
                table: "PregnancyDetails",
                column: "PregnantWomanId",
                principalTable: "PregnantWomen",
                principalColumn: "PregnantWomanId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
