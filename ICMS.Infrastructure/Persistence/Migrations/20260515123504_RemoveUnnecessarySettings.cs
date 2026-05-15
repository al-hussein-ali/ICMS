using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ICMS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnnecessarySettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DataType", "Description", "Key", "Value" },
                values: new object[] { "int", "Days after scheduled date before flagging as overdue", "Clinical.OverdueGracePeriodDays", "7" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DataType", "Description", "Key", "Value" },
                values: new object[] { "bool", "Enable automated tracking of missed primary doses", "Clinical.MissedDoseCheckEnabled", "true" });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Id", "Category", "DataType", "Description", "Key", "Value" },
                values: new object[,]
                {
                    { 4, "Clinical", "int", "Days after scheduled date before flagging as overdue", "Clinical.OverdueGracePeriodDays", "7" },
                    { 5, "Security", "int", "Days before requiring a password reset", "Security.PasswordExpiryDays", "90" }
                });
        }
    }
}
