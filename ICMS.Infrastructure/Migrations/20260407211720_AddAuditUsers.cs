using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "VisitDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "PregnancyDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Newborns",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ImmunizationRecords",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VisitDetails_UserId",
                table: "VisitDetails",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PregnancyDetails_UserId",
                table: "PregnancyDetails",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Newborns_UserId",
                table: "Newborns",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationRecords_UserId",
                table: "ImmunizationRecords",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationRecords_Users_UserId",
                table: "ImmunizationRecords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Newborns_Users_UserId",
                table: "Newborns",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PregnancyDetails_Users_UserId",
                table: "PregnancyDetails",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_UserId",
                table: "Transactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitDetails_Users_UserId",
                table: "VisitDetails",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationRecords_Users_UserId",
                table: "ImmunizationRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Newborns_Users_UserId",
                table: "Newborns");

            migrationBuilder.DropForeignKey(
                name: "FK_PregnancyDetails_Users_UserId",
                table: "PregnancyDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_UserId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitDetails_Users_UserId",
                table: "VisitDetails");

            migrationBuilder.DropIndex(
                name: "IX_VisitDetails_UserId",
                table: "VisitDetails");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_PregnancyDetails_UserId",
                table: "PregnancyDetails");

            migrationBuilder.DropIndex(
                name: "IX_Newborns_UserId",
                table: "Newborns");

            migrationBuilder.DropIndex(
                name: "IX_ImmunizationRecords_UserId",
                table: "ImmunizationRecords");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "VisitDetails");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PregnancyDetails");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Newborns");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ImmunizationRecords");
        }
    }
}
