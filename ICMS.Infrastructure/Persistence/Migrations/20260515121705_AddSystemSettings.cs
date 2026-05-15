using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ICMS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DataType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Id", "Category", "DataType", "Description", "Key", "Value" },
                values: new object[,]
                {
                    { 1, "Inventory", "int", "Days before expiry to trigger a notification", "Inventory.ExpiryThresholdDays", "30" },
                    { 2, "Communication", "time", "Daily time to send automated health advisories", "Advisory.DailyBroadcastTime", "09:00" },
                    { 3, "Clinical", "bool", "Enable automated tracking of missed primary doses", "Clinical.MissedDoseCheckEnabled", "true" },
                    { 4, "Clinical", "int", "Days after scheduled date before flagging as overdue", "Clinical.OverdueGracePeriodDays", "7" },
                    { 5, "Security", "int", "Days before requiring a password reset", "Security.PasswordExpiryDays", "90" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemSettings");
        }
    }
}
