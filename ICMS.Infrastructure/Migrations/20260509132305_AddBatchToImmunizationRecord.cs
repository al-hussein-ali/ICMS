using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBatchToImmunizationRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BatchId",
                table: "ImmunizationRecords",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationRecords_BatchId",
                table: "ImmunizationRecords",
                column: "BatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationRecords_Batches_BatchId",
                table: "ImmunizationRecords",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationRecords_Batches_BatchId",
                table: "ImmunizationRecords");

            migrationBuilder.DropIndex(
                name: "IX_ImmunizationRecords_BatchId",
                table: "ImmunizationRecords");

            migrationBuilder.DropColumn(
                name: "BatchId",
                table: "ImmunizationRecords");
        }
    }
}
