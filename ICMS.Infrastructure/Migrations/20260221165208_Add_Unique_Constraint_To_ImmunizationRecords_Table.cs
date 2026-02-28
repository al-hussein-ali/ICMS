using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Unique_Constraint_To_ImmunizationRecords_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationRecords_IndividualId_DoseId",
                table: "ImmunizationRecords",
                columns: new[] { "IndividualId", "DoseId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ImmunizationRecords_IndividualId_DoseId",
                table: "ImmunizationRecords");
        }
    }
}
