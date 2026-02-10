using ICMS.Domain.Enums;
using ICMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Entites
{
    public class VaccinatedIndividual : BaseEntity<int>
    {
        private readonly List<ImmunizationRecord> _immunizationRecords = new();
        public IReadOnlyList<ImmunizationRecord> ImmunizationRecords => _immunizationRecords.AsReadOnly();

        public string CardNumber { get; private set; } = string.Empty;
        public string Directorate { get; private set; } = string.Empty;
        public string Area { get; private set; } = string.Empty;
        public string Neighborhood { get; private set; } = string.Empty;
        public int? UserId { get; private set; }
        public int PersonId { get; private set; }
        public User? User { get; private set; }
        public Person Person { get; private set; }


        private VaccinatedIndividual()
        {
        }


        public static VaccinatedIndividual Create(string cardNumber, string directorate, string area, string neighborhood)
        {
            if (string.IsNullOrWhiteSpace(cardNumber) || string.IsNullOrWhiteSpace(directorate) || string.IsNullOrWhiteSpace(area) || string.IsNullOrWhiteSpace(neighborhood))
            {
                throw new DomainException("All fields are required!");
            }


            return new VaccinatedIndividual
            {
                CardNumber = cardNumber,
                Directorate = directorate,
                Area = area,
                Neighborhood = neighborhood,
            };

        }

        public void AssignPerson(Person person)
        {
            if (person == null) throw new DomainException("Person is required");
            if (Person != null) throw new DomainException("Person already assigned");

            Person = person;
        }

        public void AssignExistingPersonById(int personId)
        {
            if (personId == 0)
                throw new DomainException("Person Id cannot be zero");

            if (PersonId != 0)
                throw new DomainException("Person id mismatch");

            PersonId = personId;
        }

        public void AssignUser(User user)
        {
            if (user == null) throw new DomainException("User is required");
            if (User != null) throw new DomainException("User already assigned");
            if (user.Id != 0 && UserId.HasValue && user.Id != UserId.Value) throw new DomainException("User id mismatch");

            User = user;
        }

        public void AssignExistingUserById(int userId)
        {
            if (userId <= 0) throw new DomainException("Invalid user id");
            if (UserId.HasValue) throw new DomainException("This record is already linked to a user");

            UserId = userId;
        }

        public void UpdateIndividualInfo(string cardNumber, string directorate, string area, string neighborhood,
            string firstName, string secondName, string? thirdName, string lastName, Gender gender, DateOnly dateOfBirth,string phoneNumber)
        {
            this.CardNumber = cardNumber;
            this.Directorate = directorate;
            this.Area = area;
            this.Neighborhood = neighborhood;

            Person.UpdatePersonInfo(firstName,secondName,thirdName,lastName,gender,dateOfBirth);
            Person.ChangeContactInfo(phoneNumber);
        }

        public void TakeDose(int doseId, DateOnly vaccinationDate, string takenIn, int? fieldVisitId = null, string? notes = null)
        {
            if (doseId <= 0)
                throw new DomainException("Invalid Dose Id!");

            if (string.IsNullOrEmpty(takenIn))
                throw new DomainException("A required field is missing!");

            if (_immunizationRecords.Any(ir => ir.IndividualId == Id && ir.DoseId == doseId))
                throw new InvalidDoubleDoseException("This dose was taken already");


            _immunizationRecords.Add(ImmunizationRecord.Create(this.Id, doseId, vaccinationDate, takenIn, fieldVisitId, notes));
        }

    }
}
