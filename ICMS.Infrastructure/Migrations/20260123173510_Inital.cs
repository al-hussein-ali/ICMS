using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ICMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Inital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FieldVisits",
                columns: table => new
                {
                    FieldVisitId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VisitDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TargetedLocation = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldVisits", x => x.FieldVisitId);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    PersonId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SecondName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ThirdName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    LastName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('utc', NOW())"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.PersonId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Vaccines",
                columns: table => new
                {
                    VaccineId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VaccineName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    VaccineCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    TotalDosages = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vaccines", x => x.VaccineId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    PersonId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Doses",
                columns: table => new
                {
                    DoseId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VaccineId = table.Column<int>(type: "integer", nullable: false),
                    DoseName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DoseOrder = table.Column<byte>(type: "smallint", nullable: false),
                    RecommendedAgeGroup = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RecommendedAgeInMonths = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doses", x => x.DoseId);
                    table.ForeignKey(
                        name: "FK_Doses_Vaccines_VaccineId",
                        column: x => x.VaccineId,
                        principalTable: "Vaccines",
                        principalColumn: "VaccineId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FieldVisitUsers",
                columns: table => new
                {
                    FieldVisitId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldVisitUsers", x => new { x.FieldVisitId, x.UserId });
                    table.ForeignKey(
                        name: "FK_FieldVisitUsers_FieldVisits_FieldVisitId",
                        column: x => x.FieldVisitId,
                        principalTable: "FieldVisits",
                        principalColumn: "FieldVisitId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FieldVisitUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HealthAdvisories",
                columns: table => new
                {
                    HealthAdvisoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Target = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('utc', NOW())"),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthAdvisories", x => x.HealthAdvisoryId);
                    table.ForeignKey(
                        name: "FK_HealthAdvisories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PregnantWomen",
                columns: table => new
                {
                    PregnantWomanId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CurrentAddress = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    AgeRange = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PregnancyCount = table.Column<byte>(type: "smallint", nullable: false),
                    BloodGroup = table.Column<string>(type: "text", nullable: false),
                    RhFactor = table.Column<string>(type: "text", nullable: false),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PregnantWomen", x => x.PregnantWomanId);
                    table.ForeignKey(
                        name: "FK_PregnantWomen_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PregnantWomen_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VaccinatedIndividuals",
                columns: table => new
                {
                    VaccinatedIndividualId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CardNumber = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false),
                    Directorate = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Area = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Neighborhood = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    PersonId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaccinatedIndividuals", x => x.VaccinatedIndividualId);
                    table.ForeignKey(
                        name: "FK_VaccinatedIndividuals_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VaccinatedIndividuals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Batches",
                columns: table => new
                {
                    BatchId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DoseId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CountryOfOrigin = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CookNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalQuantity = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batches", x => x.BatchId);
                    table.ForeignKey(
                        name: "FK_Batches_Doses_DoseId",
                        column: x => x.DoseId,
                        principalTable: "Doses",
                        principalColumn: "DoseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Batches_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PregnancyDetails",
                columns: table => new
                {
                    PregnancyDetailsId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LastMenstrualPeriodDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DeliveryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    VisitsCount = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                    PregnancyType = table.Column<string>(type: "text", nullable: false),
                    BirthNature = table.Column<string>(type: "text", nullable: false),
                    BirthLocationType = table.Column<string>(type: "text", nullable: false),
                    BirthLocationDetails = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    BirthNatureReason = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    PregnancyComplications = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Interferences = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    NewbornCount = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                    IsPregnancyDone = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ComplicationsDuringChildbirth = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PostpartumComplications = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PregnantWomanId = table.Column<int>(type: "integer", nullable: false),
                    PreviousPregnancyComplicationsId = table.Column<int>(type: "integer", nullable: true),
                    PreviousPostartumComplicationsId = table.Column<int>(type: "integer", nullable: true),
                    PreviousPregnancyDeliveryComplicationsId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PregnancyDetails", x => x.PregnancyDetailsId);
                    table.ForeignKey(
                        name: "FK_PregnancyDetails_PregnantWomen_PregnantWomanId",
                        column: x => x.PregnantWomanId,
                        principalTable: "PregnantWomen",
                        principalColumn: "PregnantWomanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImmunizationRecords",
                columns: table => new
                {
                    ImmunizationRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    IndividualId = table.Column<int>(type: "integer", nullable: false),
                    DoseId = table.Column<int>(type: "integer", nullable: false),
                    FieldVisitId = table.Column<int>(type: "integer", nullable: true),
                    VaccinationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TakenIn = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmunizationRecords", x => x.ImmunizationRecordId);
                    table.ForeignKey(
                        name: "FK_ImmunizationRecords_Doses_DoseId",
                        column: x => x.DoseId,
                        principalTable: "Doses",
                        principalColumn: "DoseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmunizationRecords_FieldVisits_FieldVisitId",
                        column: x => x.FieldVisitId,
                        principalTable: "FieldVisits",
                        principalColumn: "FieldVisitId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmunizationRecords_VaccinatedIndividuals_IndividualId",
                        column: x => x.IndividualId,
                        principalTable: "VaccinatedIndividuals",
                        principalColumn: "VaccinatedIndividualId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DoseReports",
                columns: table => new
                {
                    DoseReportId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BatchId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('utc', NOW())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoseReports", x => x.DoseReportId);
                    table.ForeignKey(
                        name: "FK_DoseReports_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoseReports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('utc', NOW())"),
                    PermissionNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SourceorDestination = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    BatchId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Newborns",
                columns: table => new
                {
                    NewbornId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PregnancyDetailsId = table.Column<int>(type: "integer", nullable: false),
                    NewbornStatus = table.Column<string>(type: "text", nullable: false),
                    NewbornWeightInGrams = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    NewbornGender = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Newborns", x => x.NewbornId);
                    table.ForeignKey(
                        name: "FK_Newborns_PregnancyDetails_PregnancyDetailsId",
                        column: x => x.PregnancyDetailsId,
                        principalTable: "PregnancyDetails",
                        principalColumn: "PregnancyDetailsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PreviousPostartumComplications",
                columns: table => new
                {
                    PreviousPostartumComplicationsId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VaginalBleeding = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PlacentaRetention = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    VaginalFistula = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PuerperalSepsis = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    NeonatalDeathWithinFirstWeek = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PregnancyDetailId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreviousPostartumComplications", x => x.PreviousPostartumComplicationsId);
                    table.ForeignKey(
                        name: "FK_PreviousPostartumComplications_PregnancyDetails_PregnancyDe~",
                        column: x => x.PregnancyDetailId,
                        principalTable: "PregnancyDetails",
                        principalColumn: "PregnancyDetailsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PreviousPregnancyComplications",
                columns: table => new
                {
                    PreviousPregnancyComplicationsId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VaginalBleedingDuringPregnancy = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    RecurrentMiscarriageMoreThanThree = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Diabetes = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Epilepsy = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    HeartDisease = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Preeclampsia = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PretermBirthBefore8Months = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PregnancyDetailId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreviousPregnancyComplications", x => x.PreviousPregnancyComplicationsId);
                    table.ForeignKey(
                        name: "FK_PreviousPregnancyComplications_PregnancyDetails_PregnancyDe~",
                        column: x => x.PregnancyDetailId,
                        principalTable: "PregnancyDetails",
                        principalColumn: "PregnancyDetailsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PreviousPregnancyDeliveryComplications",
                columns: table => new
                {
                    PreviousPregnancyDeliveryComplicationsId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CesareanSection = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AssistedDelivery = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    StillbirthOrMultipleDeaths = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PregnancyDetailId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreviousPregnancyDeliveryComplications", x => x.PreviousPregnancyDeliveryComplicationsId);
                    table.ForeignKey(
                        name: "FK_PreviousPregnancyDeliveryComplications_PregnancyDetails_Pre~",
                        column: x => x.PregnancyDetailId,
                        principalTable: "PregnancyDetails",
                        principalColumn: "PregnancyDetailsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VisitDetails",
                columns: table => new
                {
                    VisitDetailsId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PregnancyDetailsId = table.Column<int>(type: "integer", nullable: false),
                    VisitDate = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(NOW() AT TIME ZONE 'utc')::date"),
                    NextVisitDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ClinicalExaminationAndObservation = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    WeightInKilo = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    BloodPressure = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    APPInUrineTest = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    OGTTInUrineTest = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    FetalHeartbeat = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    FetalMovement = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FetalPosition = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    PregnancyDurationInWeeks = table.Column<int>(type: "integer", nullable: false),
                    AnaemiaOrHemoglobinType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LegsSwelling = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    VaginalBleeding = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TetanusDoseNumber = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitDetails", x => x.VisitDetailsId);
                    table.ForeignKey(
                        name: "FK_VisitDetails_PregnancyDetails_PregnancyDetailsId",
                        column: x => x.PregnancyDetailsId,
                        principalTable: "PregnancyDetails",
                        principalColumn: "PregnancyDetailsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Batches_DoseId",
                table: "Batches",
                column: "DoseId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_UserId",
                table: "Batches",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DoseReports_BatchId",
                table: "DoseReports",
                column: "BatchId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoseReports_UserId",
                table: "DoseReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Doses_DoseName",
                table: "Doses",
                column: "DoseName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doses_VaccineId",
                table: "Doses",
                column: "VaccineId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldVisitUsers_UserId",
                table: "FieldVisitUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthAdvisories_UserId",
                table: "HealthAdvisories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationRecords_DoseId",
                table: "ImmunizationRecords",
                column: "DoseId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationRecords_FieldVisitId",
                table: "ImmunizationRecords",
                column: "FieldVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationRecords_IndividualId",
                table: "ImmunizationRecords",
                column: "IndividualId");

            migrationBuilder.CreateIndex(
                name: "IX_Newborns_PregnancyDetailsId",
                table: "Newborns",
                column: "PregnancyDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_People_PhoneNumber_FirstName_LastName_DateOfBirth",
                table: "People",
                columns: new[] { "PhoneNumber", "FirstName", "LastName", "DateOfBirth" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PregnancyDetails_PregnantWomanId",
                table: "PregnancyDetails",
                column: "PregnantWomanId");

            migrationBuilder.CreateIndex(
                name: "IX_PregnancyDetails_PreviousPostartumComplicationsId",
                table: "PregnancyDetails",
                column: "PreviousPostartumComplicationsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PregnancyDetails_PreviousPregnancyComplicationsId",
                table: "PregnancyDetails",
                column: "PreviousPregnancyComplicationsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PregnancyDetails_PreviousPregnancyDeliveryComplicationsId",
                table: "PregnancyDetails",
                column: "PreviousPregnancyDeliveryComplicationsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PregnantWomen_PersonId",
                table: "PregnantWomen",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PregnantWomen_UserId",
                table: "PregnantWomen",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VaccinatedIndividuals_People_Users",
                table: "PregnantWomen",
                columns: new[] { "PersonId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_PreviousPostartumComplications_PregnancyDetailId",
                table: "PreviousPostartumComplications",
                column: "PregnancyDetailId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreviousPregnancyComplications_PregnancyDetailId",
                table: "PreviousPregnancyComplications",
                column: "PregnancyDetailId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreviousPregnancyDeliveryComplications_PregnancyDetailId",
                table: "PreviousPregnancyDeliveryComplications",
                column: "PregnancyDetailId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BatchId",
                table: "Transactions",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PersonId",
                table: "Users",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VaccinatedIndividuals_CardNumber",
                table: "VaccinatedIndividuals",
                column: "CardNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VaccinatedIndividuals_People_Users1",
                table: "VaccinatedIndividuals",
                columns: new[] { "PersonId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_VaccinatedIndividuals_PersonId",
                table: "VaccinatedIndividuals",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VaccinatedIndividuals_UserId",
                table: "VaccinatedIndividuals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Vaccines_VaccineCode",
                table: "Vaccines",
                column: "VaccineCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitDetails_PregnancyDetailsId",
                table: "VisitDetails",
                column: "PregnancyDetailsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoseReports");

            migrationBuilder.DropTable(
                name: "FieldVisitUsers");

            migrationBuilder.DropTable(
                name: "HealthAdvisories");

            migrationBuilder.DropTable(
                name: "ImmunizationRecords");

            migrationBuilder.DropTable(
                name: "Newborns");

            migrationBuilder.DropTable(
                name: "PreviousPostartumComplications");

            migrationBuilder.DropTable(
                name: "PreviousPregnancyComplications");

            migrationBuilder.DropTable(
                name: "PreviousPregnancyDeliveryComplications");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "VisitDetails");

            migrationBuilder.DropTable(
                name: "FieldVisits");

            migrationBuilder.DropTable(
                name: "VaccinatedIndividuals");

            migrationBuilder.DropTable(
                name: "Batches");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "PregnancyDetails");

            migrationBuilder.DropTable(
                name: "Doses");

            migrationBuilder.DropTable(
                name: "PregnantWomen");

            migrationBuilder.DropTable(
                name: "Vaccines");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "People");
        }
    }
}
