using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RestrictPregnancyDetailsDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
//            migrationBuilder.DropForeignKey(
//                name: "FK_PregnancyDetails_PregnantWomen_PregnantWomanId",
//                table: "PregnancyDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_PregnancyDetails_PregnantWomen_PregnantWomanId",
                table: "PregnancyDetails",
                column: "PregnantWomanId",
                principalTable: "PregnantWomen",
                principalColumn: "PregnantWomanId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PregnancyDetails_PregnantWomen_PregnantWomanId",
                table: "PregnancyDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_PregnancyDetails_PregnantWomen_PregnantWomanId",
                table: "PregnancyDetails",
                column: "PregnantWomanId",
                principalTable: "PregnantWomen",
                principalColumn: "PregnantWomanId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
