using ICMS.Domain.Entites.Geography;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ICMS.Infrastructure.Persistence.Data
{
    public static class ModelBuilderExtensions
    {
        public static void SeedGeographicalData(this ModelBuilder modelBuilder)
        {
            // Seed Governorates (Republic of Yemen - 22 Governorates)
            modelBuilder.Entity<Governorate>().HasData(
                new { Id = 1, Name = "صنعاء" },
                new { Id = 2, Name = "عدن" },
                new { Id = 3, Name = "تعز" },
                new { Id = 4, Name = "الحديدة" },
                new { Id = 5, Name = "حضرموت" },
                new { Id = 6, Name = "إب" },
                new { Id = 7, Name = "أبين" },
                new { Id = 8, Name = "البيضاء" },
                new { Id = 9, Name = "لحج" },
                new { Id = 10, Name = "مأرب" },
                new { Id = 11, Name = "شبوة" },
                new { Id = 12, Name = "الجوف" },
                new { Id = 13, Name = "المهرة" },
                new { Id = 14, Name = "المحويت" },
                new { Id = 15, Name = "صعدة" },
                new { Id = 16, Name = "حجة" },
                new { Id = 17, Name = "الضالع" },
                new { Id = 18, Name = "عمران" },
                new { Id = 19, Name = "ذمار" },
                new { Id = 20, Name = "ريمة" },
                new { Id = 21, Name = "سقطرى" },
                new { Id = 22, Name = "أمانة العاصمة" }
            );

            // Seed Directorates (Focus on Amanat Al-Asimah & Hadhramaut)
            modelBuilder.Entity<Directorate>().HasData(
                // Amanat Al-Asimah (ID 22)
                new { Id = 1, Name = "السبعين", GovernorateId = 22 },
                new { Id = 2, Name = "الوحدة", GovernorateId = 22 },
                new { Id = 3, Name = "التحرير", GovernorateId = 22 },
                new { Id = 4, Name = "معين", GovernorateId = 22 },
                new { Id = 5, Name = "الصافية", GovernorateId = 22 },
                new { Id = 6, Name = "آزال", GovernorateId = 22 },
                new { Id = 7, Name = "صنعاء القديمة", GovernorateId = 22 },
                new { Id = 8, Name = "شعوب", GovernorateId = 22 },
                new { Id = 9, Name = "الثورة", GovernorateId = 22 },
                new { Id = 10, Name = "بني الحارث", GovernorateId = 22 },

                // Hadhramaut (ID 5)
                new { Id = 11, Name = "مدينة المكلا", GovernorateId = 5 },
                new { Id = 12, Name = "أرياف المكلا", GovernorateId = 5 },
                new { Id = 13, Name = "الشحر", GovernorateId = 5 },
                new { Id = 14, Name = "سيئون", GovernorateId = 5 },
                new { Id = 15, Name = "غيل باوزير", GovernorateId = 5 }
            );

            // Seed Neighborhoods
            modelBuilder.Entity<Neighborhood>().HasData(
                // منطقة فوة
                new { Id = 1, Name = "فوة-المتضررين", DirectorateId = 11 },
                new { Id = 2, Name = "فوة-ابن سيناء", DirectorateId = 11 },
                new { Id = 3, Name = "فوة-المساكن", DirectorateId = 11 },
                new { Id = 4, Name = "فوة-الإنشاءات", DirectorateId = 11 },
                new { Id = 5, Name = "فوة-القديمة", DirectorateId = 11 },

                // منطقة الشرج
                new { Id = 6, Name = "الشرج-حي العمال", DirectorateId = 11 },
                new { Id = 7, Name = "الشرج-باعبود", DirectorateId = 11 },
                new { Id = 8, Name = "الشرج-المنورة", DirectorateId = 11 },
                new { Id = 9, Name = "الشرج-حي أكتوبر", DirectorateId = 11 },

                // منطقة الديس
                new { Id = 10, Name = "الديس-شعب البادية", DirectorateId = 11 },
                new { Id = 11, Name = "الديس-الغويزي", DirectorateId = 11 },
                new { Id = 12, Name = "الديس-حي 14 أكتوبر", DirectorateId = 11 },
                new { Id = 13, Name = "الديس-جول الشفاء", DirectorateId = 11 },

                // المكلا القديمة
                new { Id = 14, Name = "المكلا-حي الشهيد", DirectorateId = 11 },
                new { Id = 15, Name = "المكلا-حي السلام", DirectorateId = 11 },
                new { Id = 16, Name = "المكلا-حي الصيادين", DirectorateId = 11 },

                // منطقة السبعين - أمانة العاصمة
                new { Id = 17, Name = "السبعين-حي الوحدة", DirectorateId = 1 },
                new { Id = 18, Name = "السبعين-حي القادسية", DirectorateId = 1 },
                new { Id = 19, Name = "السبعين-حي النصر", DirectorateId = 1 }
            );

            // Seed Sub-Neighborhoods
            modelBuilder.Entity<SubNeighborhood>().HasData(
                // مربعات فوة المتضررين
                new { Id = 1, Name = "المتضررين-وحدة الصديق", NeighborhoodId = 1 },
                new { Id = 2, Name = "المتضررين-وحدة الربوة", NeighborhoodId = 1 },
                new { Id = 3, Name = "المتضررين-المنطقة المرتفعة", NeighborhoodId = 1 },

                // مربعات فوة ابن سيناء
                new { Id = 4, Name = "ابن سيناء-مربع السكن الجامعي", NeighborhoodId = 2 },
                new { Id = 5, Name = "ابن سيناء-حي الكوادر", NeighborhoodId = 2 },
                new { Id = 6, Name = "ابن سيناء-منطقة المستشفى", NeighborhoodId = 2 },

                // مربعات الشرج حي العمال
                new { Id = 7, Name = "حي العمال-مربع الورش", NeighborhoodId = 6 },
                new { Id = 8, Name = "حي العمال-عمارة باجرش", NeighborhoodId = 6 },

                // مربعات الديس شعب البادية
                new { Id = 9, Name = "شعب البادية-حارة الروضة", NeighborhoodId = 10 },
                new { Id = 10, Name = "شعب البادية-جول الغليلة", NeighborhoodId = 10 },

                // مربعات المكلا القديمة
                new { Id = 11, Name = "حي الشهيد-سكة يعقوب", NeighborhoodId = 14 },
                new { Id = 12, Name = "حي الشهيد-حارة البلاد", NeighborhoodId = 14 },
                new { Id = 13, Name = "حي السلام-خلف البريد", NeighborhoodId = 15 },

                // Additional blocks for other neighborhoods
                new { Id = 14, Name = "المساكن-المربع الأول", NeighborhoodId = 3 },
                new { Id = 15, Name = "الإنشاءات-المربع التقني", NeighborhoodId = 4 },
                new { Id = 16, Name = "فوة القديمة-حارة الساحل", NeighborhoodId = 5 },
                new { Id = 17, Name = "باعبود-شارع الفنار", NeighborhoodId = 7 },
                new { Id = 18, Name = "المنورة-المربع الغربي", NeighborhoodId = 8 },
                new { Id = 19, Name = "حي أكتوبر-بلوك 5", NeighborhoodId = 9 },
                new { Id = 20, Name = "الغويزي-بجانب القلعة", NeighborhoodId = 11 },
                new { Id = 21, Name = "14 أكتوبر-بلوك 2", NeighborhoodId = 12 },
                new { Id = 22, Name = "جول الشفاء-وحدة الهدى", NeighborhoodId = 13 },
                new { Id = 23, Name = "حي الصيادين-المربع البحري", NeighborhoodId = 16 },

                // مربعات السبعين
                new { Id = 24, Name = "حي الوحدة-المربع أ", NeighborhoodId = 17 },
                new { Id = 25, Name = "حي القادسية-المربع 1", NeighborhoodId = 18 },
                new { Id = 26, Name = "حي النصر-المربع الشمالي", NeighborhoodId = 19 }
            );
        }
        public static void SeedIdentityData(this ModelBuilder modelBuilder)
        {
            // Seed Roles
            modelBuilder.Entity<ICMS.Domain.Entites.Identity.Role>().HasData(
                new { Id = 1001, RoleName = ICMS.Domain.Constants.Roles.Admin },
                new { Id = 1002, RoleName = ICMS.Domain.Constants.Roles.InventoryManager },
                new { Id = 1003, RoleName = ICMS.Domain.Constants.Roles.VaccinationManager },
                new { Id = 1004, RoleName = ICMS.Domain.Constants.Roles.ReproductiveHealthManager },
                new { Id = 1005, RoleName = ICMS.Domain.Constants.Roles.FieldVisitWorker },
                new { Id = 1006, RoleName = ICMS.Domain.Constants.Roles.VaccinatedIndividual },
                new { Id = 1007, RoleName = ICMS.Domain.Constants.Roles.PregnantWoman }
            );

            // Seed Admin Person
            modelBuilder.Entity<ICMS.Domain.Entites.Identity.Person>().HasData(
                new
                {
                    Id = 999,
                    FirstName = "System",
                    SecondName = "Admin",
                    LastName = "User",
                    Gender = ICMS.Domain.Enums.Gender.Male,
                    DateOfBirth = new DateOnly(1990, 1, 1),
                    PhoneNumber = "777777777",
                    CreatedAt = new DateTime(2026, 4, 21, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false
                }
            );

            // Seed Admin User
            // Password is 'Admin@123'
            modelBuilder.Entity<ICMS.Domain.Entites.Identity.User>().HasData(
                new
                {
                    Id = 999,
                    UserName = "admin",
                    PasswordHash = "$2a$11$tulMkZYZGUHXu/UNv3EJB.5WfDjvJEaYWjH8UZYHrGAJ/UjBvBlZq",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 4, 21, 0, 0, 0, DateTimeKind.Utc),
                    PersonId = 999
                }
            );

            // Assign Admin Role to Admin User
            modelBuilder.Entity<ICMS.Domain.Entites.Identity.UserRole>().HasData(
                new { UserId = 999, RoleId = 1001 }
            );
        }

        public static void SeedVaccineData(this ModelBuilder modelBuilder)
        {
            // Seed Vaccines
            modelBuilder.Entity<Vaccine>().HasData(
                new { Id = 1, VaccineName = "BCG", VaccineCode = "BCG", TotalDosages = (byte)1, MinEligibleAgeInMonths = 0, MaxEligibleAgeInMonths = 1, IsActive = true, Audience = TargetAudience.InfantRoutine },
                new { Id = 2, VaccineName = "Oral Polio (OPV)", VaccineCode = "OPV", TotalDosages = (byte)4, MinEligibleAgeInMonths = 0, MaxEligibleAgeInMonths = 60, IsActive = true, Audience = TargetAudience.InfantRoutine },
                new { Id = 3, VaccineName = "Pentavalent", VaccineCode = "PENTA", TotalDosages = (byte)3, MinEligibleAgeInMonths = 1, MaxEligibleAgeInMonths = 12, IsActive = true, Audience = TargetAudience.InfantRoutine },
                new { Id = 4, VaccineName = "Rotavirus", VaccineCode = "ROTA", TotalDosages = (byte)2, MinEligibleAgeInMonths = 1, MaxEligibleAgeInMonths = 8, IsActive = true, Audience = TargetAudience.InfantRoutine },
                new { Id = 5, VaccineName = "Pneumococcal (PCV)", VaccineCode = "PCV", TotalDosages = (byte)3, MinEligibleAgeInMonths = 1, MaxEligibleAgeInMonths = 24, IsActive = true, Audience = TargetAudience.InfantRoutine },
                new { Id = 6, VaccineName = "Measles & Rubella (MR)", VaccineCode = "MR", TotalDosages = (byte)2, MinEligibleAgeInMonths = 9, MaxEligibleAgeInMonths = 60, IsActive = true, Audience = TargetAudience.InfantRoutine },
                new { Id = 7, VaccineName = "Inactivated Polio (IPV)", VaccineCode = "IPV", TotalDosages = (byte)1, MinEligibleAgeInMonths = 3, MaxEligibleAgeInMonths = 12, IsActive = true, Audience = TargetAudience.InfantRoutine },
                new { Id = 8, VaccineName = "Vitamin A", VaccineCode = "VITA", TotalDosages = (byte)2, MinEligibleAgeInMonths = 6, MaxEligibleAgeInMonths = 60, IsActive = true, Audience = TargetAudience.InfantRoutine },
                new { Id = 9, VaccineName = "Tetanus Toxoid", VaccineCode = "TT", TotalDosages = (byte)5, MinEligibleAgeInMonths = 180, MaxEligibleAgeInMonths = 600, IsActive = true, Audience = TargetAudience.GeneralAdult }
            );

            // Seed Doses
            modelBuilder.Entity<Dose>().HasData(
                // BCG
                new { Id = 1, VaccineId = 1, DoseName = "{\"en\":\"BCG\",\"ar\":\"بي سي جي\"}", DoseOrder = (byte)1, RecommendedAgeInWeeks = 0, RecommendedAgeGroup = "atBirth", IsPrimary = true },
                // OPV
                new { Id = 2, VaccineId = 2, DoseName = "{\"en\":\"OPV 0\",\"ar\":\"شلل الأطفال الفموي 0\"}", DoseOrder = (byte)1, RecommendedAgeInWeeks = 0, RecommendedAgeGroup = "atBirth", IsPrimary = true },
                new { Id = 3, VaccineId = 2, DoseName = "{\"en\":\"OPV 1\",\"ar\":\"شلل الأطفال الفموي 1\"}", DoseOrder = (byte)2, RecommendedAgeInWeeks = 8, RecommendedAgeGroup = "2months", IsPrimary = true },
                new { Id = 4, VaccineId = 2, DoseName = "{\"en\":\"OPV 2\",\"ar\":\"شلل الأطفال الفموي 2\"}", DoseOrder = (byte)3, RecommendedAgeInWeeks = 16, RecommendedAgeGroup = "4months", IsPrimary = true },
                new { Id = 5, VaccineId = 2, DoseName = "{\"en\":\"OPV 3\",\"ar\":\"شلل الأطفال الفموي 3\"}", DoseOrder = (byte)4, RecommendedAgeInWeeks = 24, RecommendedAgeGroup = "6months", IsPrimary = true },
                // PENTA
                new { Id = 6, VaccineId = 3, DoseName = "{\"en\":\"Penta 1\",\"ar\":\"الخماسي 1\"}", DoseOrder = (byte)1, RecommendedAgeInWeeks = 8, RecommendedAgeGroup = "2months", IsPrimary = true },
                new { Id = 7, VaccineId = 3, DoseName = "{\"en\":\"Penta 2\",\"ar\":\"الخماسي 2\"}", DoseOrder = (byte)2, RecommendedAgeInWeeks = 16, RecommendedAgeGroup = "4months", IsPrimary = true },
                new { Id = 8, VaccineId = 3, DoseName = "{\"en\":\"Penta 3\",\"ar\":\"الخماسي 3\"}", DoseOrder = (byte)3, RecommendedAgeInWeeks = 24, RecommendedAgeGroup = "6months", IsPrimary = true },
                // ROTA
                new { Id = 9, VaccineId = 4, DoseName = "{\"en\":\"Rota 1\",\"ar\":\"الروتا 1\"}", DoseOrder = (byte)1, RecommendedAgeInWeeks = 8, RecommendedAgeGroup = "2months", IsPrimary = true },
                new { Id = 10, VaccineId = 4, DoseName = "{\"en\":\"Rota 2\",\"ar\":\"الروتا 2\"}", DoseOrder = (byte)2, RecommendedAgeInWeeks = 16, RecommendedAgeGroup = "4months", IsPrimary = true },
                // PCV
                new { Id = 11, VaccineId = 5, DoseName = "{\"en\":\"PCV 1\",\"ar\":\"المكورات الرئوية 1\"}", DoseOrder = (byte)1, RecommendedAgeInWeeks = 8, RecommendedAgeGroup = "2months", IsPrimary = true },
                new { Id = 12, VaccineId = 5, DoseName = "{\"en\":\"PCV 2\",\"ar\":\"المكورات الرئوية 2\"}", DoseOrder = (byte)2, RecommendedAgeInWeeks = 16, RecommendedAgeGroup = "4months", IsPrimary = true },
                new { Id = 13, VaccineId = 5, DoseName = "{\"en\":\"PCV 3\",\"ar\":\"المكورات الرئوية 3\"}", DoseOrder = (byte)3, RecommendedAgeInWeeks = 24, RecommendedAgeGroup = "6months", IsPrimary = true },
                // MR
                new { Id = 14, VaccineId = 6, DoseName = "{\"en\":\"MR 1\",\"ar\":\"الحصبة والحصبة الألمانية 1\"}", DoseOrder = (byte)1, RecommendedAgeInWeeks = 36, RecommendedAgeGroup = "9months", IsPrimary = true },
                new { Id = 15, VaccineId = 6, DoseName = "{\"en\":\"MR 2\",\"ar\":\"الحصبة والحصبة الألمانية 2\"}", DoseOrder = (byte)2, RecommendedAgeInWeeks = 72, RecommendedAgeGroup = "18months", IsPrimary = true },
                // IPV
                new { Id = 16, VaccineId = 7, DoseName = "{\"en\":\"IPV\",\"ar\":\"شلل الأطفال غير النشط\"}", DoseOrder = (byte)1, RecommendedAgeInWeeks = 16, RecommendedAgeGroup = "4months", IsPrimary = true },
                // VITA
                new { Id = 17, VaccineId = 8, DoseName = "{\"en\":\"Vit A 1\",\"ar\":\"فيتامين أ 1\"}", DoseOrder = (byte)1, RecommendedAgeInWeeks = 24, RecommendedAgeGroup = "6months", IsPrimary = true },
                new { Id = 18, VaccineId = 8, DoseName = "{\"en\":\"Vit A 2\",\"ar\":\"فيتامين أ 2\"}", DoseOrder = (byte)2, RecommendedAgeInWeeks = 48, RecommendedAgeGroup = "12months", IsPrimary = true },
                // TT (Reproductive Age: Females 15-49)
                new { Id = 19, VaccineId = 9, DoseName = "{\"en\":\"TT 1\",\"ar\":\"كزاز 1\"}", DoseOrder = (byte)1, RecommendedAgeInWeeks = 720, RecommendedAgeGroup = "female15_49", IsPrimary = true },
                new { Id = 20, VaccineId = 9, DoseName = "{\"en\":\"TT 2\",\"ar\":\"كزاز 2\"}", DoseOrder = (byte)2, RecommendedAgeInWeeks = 724, RecommendedAgeGroup = "female15_49", IsPrimary = true },
                new { Id = 21, VaccineId = 9, DoseName = "{\"en\":\"TT 3\",\"ar\":\"كزاز 3\"}", DoseOrder = (byte)3, RecommendedAgeInWeeks = 748, RecommendedAgeGroup = "female15_49", IsPrimary = true },
                new { Id = 22, VaccineId = 9, DoseName = "{\"en\":\"TT 4\",\"ar\":\"كزاز 4\"}", DoseOrder = (byte)4, RecommendedAgeInWeeks = 796, RecommendedAgeGroup = "female15_49", IsPrimary = true },
                new { Id = 23, VaccineId = 9, DoseName = "{\"en\":\"TT 5\",\"ar\":\"كزاز 5\"}", DoseOrder = (byte)5, RecommendedAgeInWeeks = 844, RecommendedAgeGroup = "female15_49", IsPrimary = true }
            );
        }

        public static void SeedSystemSettings(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SystemSetting>().HasData(
                new { Id = 1, Key = "Inventory.ExpiryThresholdDays", Value = "30", Category = "Inventory", Description = "Days before expiry to trigger a notification", DataType = "int" },
                new { Id = 2, Key = "Advisory.DailyBroadcastTime", Value = "09:00", Category = "Communication", Description = "Daily time to send automated health advisories", DataType = "time" },
                new { Id = 3, Key = "Clinical.OverdueGracePeriodDays", Value = "7", Category = "Clinical", Description = "Days after scheduled date before flagging as overdue", DataType = "int" }
            );
        }
    }
}
