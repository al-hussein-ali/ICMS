using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ICMS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFetalDetailsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Notifications",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "TargetUrl",
                table: "Notifications",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Notifications",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "JobId",
                table: "Notifications",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "FetalDetails",
                columns: table => new
                {
                    FetalDetailId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VisitDetailsId = table.Column<int>(type: "integer", nullable: false),
                    FetusLabel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FetalHeartbeat = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    FetalHeartbeatValue = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FetalMovement = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FetalPosition = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FetalDetails", x => x.FetalDetailId);
                    table.ForeignKey(
                        name: "FK_FetalDetails_VisitDetails_VisitDetailsId",
                        column: x => x.VisitDetailsId,
                        principalTable: "VisitDetails",
                        principalColumn: "VisitDetailsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FetalDetails_VisitDetailsId",
                table: "FetalDetails",
                column: "VisitDetailsId");

            // Migrate data: copy existing values from VisitDetails to FetalDetails
            migrationBuilder.Sql(@"
                INSERT INTO ""FetalDetails"" (""FetusLabel"", ""FetalHeartbeat"", ""FetalHeartbeatValue"", ""FetalMovement"", ""FetalPosition"", ""VisitDetailsId"")
                SELECT 'Fetus A', ""FetalHeartbeat"", ""FetalHeartbeatValue"", ""FetalMovement"", ""FetalPosition"", ""VisitDetailsId""
                FROM ""VisitDetails"";
            ");

            migrationBuilder.DropColumn(
                name: "FetalHeartbeat",
                table: "VisitDetails");

            migrationBuilder.DropColumn(
                name: "FetalHeartbeatValue",
                table: "VisitDetails");

            migrationBuilder.DropColumn(
                name: "FetalMovement",
                table: "VisitDetails");

            migrationBuilder.DropColumn(
                name: "FetalPosition",
                table: "VisitDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FetalHeartbeat",
                table: "VisitDetails",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FetalHeartbeatValue",
                table: "VisitDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FetalMovement",
                table: "VisitDetails",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FetalPosition",
                table: "VisitDetails",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            // Copy data back (first fetus labeled 'Fetus A' back to the single record columns)
            migrationBuilder.Sql(@"
                UPDATE ""VisitDetails"" vd
                SET 
                    ""FetalHeartbeat"" = fd.""FetalHeartbeat"",
                    ""FetalHeartbeatValue"" = fd.""FetalHeartbeatValue"",
                    ""FetalMovement"" = fd.""FetalMovement"",
                    ""FetalPosition"" = fd.""FetalPosition""
                FROM ""FetalDetails"" fd
                WHERE fd.""VisitDetailsId"" = vd.""VisitDetailsId"" AND fd.""FetusLabel"" = 'Fetus A';
            ");

            migrationBuilder.DropTable(
                name: "FetalDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Notifications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "TargetUrl",
                table: "Notifications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Notifications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "JobId",
                table: "Notifications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
