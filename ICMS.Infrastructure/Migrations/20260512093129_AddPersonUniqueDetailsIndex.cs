using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonUniqueDetailsIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_People_PhoneNumber_FirstName_LastName_DateOfBirth",
                table: "People",
                newName: "IX_People_UniqueDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_People_UniqueDetails",
                table: "People",
                newName: "IX_People_PhoneNumber_FirstName_LastName_DateOfBirth");
        }
    }
}
