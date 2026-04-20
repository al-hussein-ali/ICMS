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
        public const string Admin = "Admin";
        public const string VaccinationManager = "VaccinationManager";
        public const string InventoryManager = "InventoryManager";
        public const string ReproductiveHealthManager = "ReproductiveHealthManager";
        public const string VaccinatedIndividual = "VaccinatedIndividual";
        public const string PregnantWoman = "PregnantWoman";
        public const string FieldVisitWorker = "FieldVisitWorker";

        public const string StaffRoles = Admin + "," + VaccinationManager + "," + InventoryManager + "," + ReproductiveHealthManager + "," + FieldVisitWorker;
    }
}
