using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldVisitReminder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReminderSent",
                table: "FieldVisits",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderSent",
                table: "FieldVisits");
        }
    }
}
