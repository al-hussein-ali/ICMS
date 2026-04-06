using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Constants
{
    public class Roles
    {
        public static string Admin = "Admin";
        public static string VaccinationManager = "VaccinationManager";
        public static string InventoryManager = "InventoryManager";
        public static string ReproductiveHealthManager = "ReproductiveHealthManager";
        public static string VaccinatedIndividual = "VaccinatedIndividual";
        public static string PregnantWoman = "PregnantWoman";
        public static string FieldVisitWorker = "FieldVisitWorker";
    }
}
