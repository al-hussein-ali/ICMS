using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixMaternalNamingTypos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop FK and PK constraints first
            migrationBuilder.Sql(@"ALTER TABLE ""PreviousPostpartumComplications"" DROP CONSTRAINT IF EXISTS ""FK_PreviousPostartumComplications_PregnancyDetails_PregnancyDe~"";");
            migrationBuilder.Sql(@"ALTER TABLE ""PreviousPostpartumComplications"" DROP CONSTRAINT IF EXISTS ""PK_PreviousPostartumComplications"";");

            // Drop old indexes
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_PreviousPostartumComplications_PregnancyDetailId"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_PregnancyDetails_PreviousPostartumComplicationsId"";");

            // Rename the PK column (Table is already named correctly in initial migration)

            // Rename the PK column on the renamed table
            migrationBuilder.Sql(@"ALTER TABLE ""PreviousPostpartumComplications"" RENAME COLUMN ""PreviousPostartumComplicationsId"" TO ""PreviousPostpartumComplicationsId"";");

            // Rename the FK column on PregnancyDetails
            migrationBuilder.Sql(@"ALTER TABLE ""PregnancyDetails"" RENAME COLUMN ""PreviousPostartumComplicationsId"" TO ""PreviousPostpartumComplicationsId"";");

            // Re-create PK
            migrationBuilder.Sql(@"ALTER TABLE ""PreviousPostpartumComplications"" ADD CONSTRAINT ""PK_PreviousPostpartumComplications"" PRIMARY KEY (""PreviousPostpartumComplicationsId"");");

            // Re-create indexes
            migrationBuilder.Sql(@"CREATE INDEX ""IX_PreviousPostpartumComplications_PregnancyDetailId"" ON ""PreviousPostpartumComplications"" (""PregnancyDetailId"");");
            migrationBuilder.Sql(@"CREATE INDEX ""IX_PregnancyDetails_PreviousPostpartumComplicationsId"" ON ""PregnancyDetails"" (""PreviousPostpartumComplicationsId"");");

            // Re-create FK
            migrationBuilder.Sql(@"ALTER TABLE ""PreviousPostpartumComplications"" ADD CONSTRAINT ""FK_PreviousPostpartumComplications_PregnancyDetails_PregnancyD~"" FOREIGN KEY (""PregnancyDetailId"") REFERENCES ""PregnancyDetails"" (""PregnancyDetailsId"") ON DELETE RESTRICT;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop FK and PK constraints
            migrationBuilder.Sql(@"ALTER TABLE ""PreviousPostpartumComplications"" DROP CONSTRAINT IF EXISTS ""FK_PreviousPostpartumComplications_PregnancyDetails_PregnancyD~"";");
            migrationBuilder.Sql(@"ALTER TABLE ""PreviousPostpartumComplications"" DROP CONSTRAINT IF EXISTS ""PK_PreviousPostpartumComplications"";");

            // Drop indexes
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_PreviousPostpartumComplications_PregnancyDetailId"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_PregnancyDetails_PreviousPostpartumComplicationsId"";");

            // Rename columns back (Table remains correct spelling)

            // Rename columns back
            migrationBuilder.Sql(@"ALTER TABLE ""PreviousPostartumComplications"" RENAME COLUMN ""PreviousPostpartumComplicationsId"" TO ""PreviousPostartumComplicationsId"";");
            migrationBuilder.Sql(@"ALTER TABLE ""PregnancyDetails"" RENAME COLUMN ""PreviousPostpartumComplicationsId"" TO ""PreviousPostartumComplicationsId"";");

            // Re-create PK
            migrationBuilder.Sql(@"ALTER TABLE ""PreviousPostpartumComplications"" ADD CONSTRAINT ""PK_PreviousPostartumComplications"" PRIMARY KEY (""PreviousPostartumComplicationsId"");");

            // Re-create indexes
            migrationBuilder.Sql(@"CREATE INDEX ""IX_PreviousPostartumComplications_PregnancyDetailId"" ON ""PreviousPostpartumComplications"" (""PregnancyDetailId"");");
            migrationBuilder.Sql(@"CREATE INDEX ""IX_PregnancyDetails_PreviousPostartumComplicationsId"" ON ""PregnancyDetails"" (""PreviousPostartumComplicationsId"");");

            // Re-create FK
            migrationBuilder.Sql(@"ALTER TABLE ""PreviousPostpartumComplications"" ADD CONSTRAINT ""FK_PreviousPostartumComplications_PregnancyDetails_PregnancyDe~"" FOREIGN KEY (""PregnancyDetailId"") REFERENCES ""PregnancyDetails"" (""PregnancyDetailsId"") ON DELETE RESTRICT;");
        }
    }
}
