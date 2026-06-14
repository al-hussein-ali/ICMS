using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldVisitWorkers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FieldVisitWorkers",
                columns: table => new
                {
                    FieldVisitId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldVisitWorkers", x => new { x.FieldVisitId, x.UserId });
                    table.ForeignKey(
                        name: "FK_FieldVisitWorkers_FieldVisits_FieldVisitId",
                        column: x => x.FieldVisitId,
                        principalTable: "FieldVisits",
                        principalColumn: "FieldVisitId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldVisitWorkers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldVisitWorkers_UserId",
                table: "FieldVisitWorkers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldVisitWorkers");
        }
    }
}
