using ICMS.Domain.Entites.Geography;
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
                new { Id = 1, Name = "فوة-المتضررين", DirectorateId = 1 },
                new { Id = 2, Name = "فوة-ابن سيناء", DirectorateId = 1 },
                new { Id = 3, Name = "فوة-المساكن", DirectorateId = 1 },
                new { Id = 4, Name = "فوة-الإنشاءات", DirectorateId = 1 },
                new { Id = 5, Name = "فوة-القديمة", DirectorateId = 1 },

                // منطقة الشرج
                new { Id = 6, Name = "الشرج-حي العمال", DirectorateId = 1 },
                new { Id = 7, Name = "الشرج-باعبود", DirectorateId = 1 },
                new { Id = 8, Name = "الشرج-المنورة", DirectorateId = 1 },
                new { Id = 9, Name = "الشرج-حي أكتوبر", DirectorateId = 1 },

                // منطقة الديس
                new { Id = 10, Name = "الديس-شعب البادية", DirectorateId = 1 },
                new { Id = 11, Name = "الديس-الغويزي", DirectorateId = 1 },
                new { Id = 12, Name = "الديس-حي 14 أكتوبر", DirectorateId = 1 },
                new { Id = 13, Name = "الديس-جول الشفاء", DirectorateId = 1 },

                // المكلا القديمة
                new { Id = 14, Name = "المكلا-حي الشهيد", DirectorateId = 1 },
                new { Id = 15, Name = "المكلا-حي السلام", DirectorateId = 1 },
                new { Id = 16, Name = "المكلا-حي الصيادين", DirectorateId = 1 }
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
                new { Id = 13, Name = "حي السلام-خلف البريد", NeighborhoodId = 15 }
            );
        }
    }
}
