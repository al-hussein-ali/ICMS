using ICMS.Domain.Entites.Geography;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Entites.Clinical
{
    public class Vaccine : BaseEntity<int>
    {
        private readonly List<Dose> _doses = new();
        public IReadOnlyList<Dose> Doses => _doses.AsReadOnly();

        public string VaccineName { get; private set; } = string.Empty;
        public string VaccineCode { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public bool IsActive { get; private set; } = true;
        public byte TotalDosages { get; private set; }
        public ICMS.Domain.Enums.TargetAudience Audience { get; private set; }

        private Vaccine()
        {
        }

        public static Vaccine Create(string vaccineName, string vaccineCode, string? description, bool isActive, byte totalDosages, ICMS.Domain.Enums.TargetAudience audience)
        {
            if (string.IsNullOrWhiteSpace(vaccineName)) throw new DomainException("Vaccine name is required");
            if (string.IsNullOrWhiteSpace(vaccineCode)) throw new DomainException("Vaccine code is required");

            return new Vaccine
            {
                VaccineName = vaccineName,
                VaccineCode = vaccineCode,
                Description = description,
                IsActive = isActive,
                TotalDosages = totalDosages,
                Audience = audience
            };
        }

        public void AddDose(Dose dose)
        {
            if (dose == null) throw new DomainException("Dose is required.");
            if (dose.Vaccine != null) throw new DomainException("Dose already assigned to a vaccine.");

            if (Doses.Count + 1 > TotalDosages)
                throw new DomainException("The number of Doses exeeded the expected total dosages.");


            if (_doses.Any(d => d.DoseOrder == dose.DoseOrder))
                throw new DomainException("Cannot add Dose with redundunt order.");


            if (_doses.Any(d => d.DoseName.Equals(dose.DoseName,StringComparison.OrdinalIgnoreCase)))
                throw new DomainException("Dose with this name already exists.");

            _doses.Add(dose);

        }

        public void UpdateVaccineInfo(string vaccineName, string vaccineCode, string? description, bool isActive, byte totalDosages, ICMS.Domain.Enums.TargetAudience audience)
        {
            VaccineName = vaccineName;
            VaccineCode = vaccineCode;
            Description = description;
            IsActive = isActive;
            Audience = audience;

            if (totalDosages < Doses.Count)
                throw new DomainException("The Number of Total Doses is less than Dosages");

            TotalDosages = totalDosages;
        }

        public void Deactivate()
        {
            if (!IsActive)
                return;
            IsActive = false;
        }

        public void Reactivate()
        {
            if (IsActive)
                throw new DomainException("The Vaccine is already active!");

            IsActive = true;
        }
    }
}
