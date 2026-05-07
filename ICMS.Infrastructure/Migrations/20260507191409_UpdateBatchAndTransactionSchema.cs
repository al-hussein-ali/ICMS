using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBatchAndTransactionSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SourceorDestination",
                table: "Transactions",
                newName: "SourceOrDestination");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Transactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConsumedQuantity",
                table: "Batches",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ConsumedQuantity",
                table: "Batches");

            migrationBuilder.RenameColumn(
                name: "SourceOrDestination",
                table: "Transactions",
                newName: "SourceorDestination");
        }
    }
}
