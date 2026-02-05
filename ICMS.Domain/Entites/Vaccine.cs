using ICMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Entites
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

        private Vaccine()
        {
        }

        public static Vaccine Create(string vaccineName, string vaccineCode, string? description, bool isActive, byte totalDosages)
        {
            if (string.IsNullOrWhiteSpace(vaccineName)) throw new DomainException("Vaccine name is required");
            if (string.IsNullOrWhiteSpace(vaccineCode)) throw new DomainException("Vaccine code is required");

            return new Vaccine
            {
                VaccineName = vaccineName,
                VaccineCode = vaccineCode,
                Description = description,
                IsActive = isActive,
                TotalDosages = totalDosages
            };
        }

        public void AddDose(Dose dose)
        {
            if (dose == null) throw new DomainException("Dose is required");
            if (_doses.Any(d => d.Id == dose.Id)) throw new DomainException("Dose already added");

            _doses.Add(dose);
        }

        public void AssignDose(Dose dose)
        {
            if (dose == null) throw new DomainException("Dose is required");
            if (dose.Vaccine != null) throw new DomainException("Dose already assigned to a vaccine");

            dose.AssignVaccine(this);
            if (!_doses.Any(d => d.Id == dose.Id)) _doses.Add(dose);
        }
    }
}
