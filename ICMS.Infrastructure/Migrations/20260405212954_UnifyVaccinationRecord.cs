using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UnifyVaccinationRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TetanusDoseNumber",
                table: "VisitDetails");

            migrationBuilder.AddColumn<int>(
                name: "Audience",
                table: "Vaccines",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Audience",
                table: "Vaccines");

            migrationBuilder.AddColumn<byte>(
                name: "TetanusDoseNumber",
                table: "VisitDetails",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
