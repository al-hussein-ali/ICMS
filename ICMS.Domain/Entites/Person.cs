using ICMS.Domain.Enums;
using ICMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Entites
{
    public class Person : BaseEntity<int>
    {
        public string FirstName { get; private set; } = string.Empty;
        public string SecondName { get; private set; } = string.Empty;
        public string? ThirdName { get; private set; }
        public string LastName { get; private set; } = string.Empty;
        public string FullName => $"{FirstName} {SecondName} {ThirdName ?? ""} {LastName}";
        public Gender Gender { get; private set; }
        public DateOnly DateOfBirth { get; private set; }
        public string PhoneNumber { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public User? User { get; private set; }
        public PregnantWoman? PregnantWoman { get; private set; }
        public VaccinatedIndividual? VaccinatedIndividual { get; private set; }


        private Person()
        {
        }

        public static Person Create(string firstName, string secondName, string? thirdName, string lastName, Gender gender, DateOnly dateOfBirth, string phoneNumber)
        {
            return new Person
            {
                FirstName = firstName,
                SecondName = secondName,
                ThirdName = thirdName,
                LastName = lastName,
                Gender = gender,
                DateOfBirth = dateOfBirth,
                PhoneNumber = phoneNumber,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void MarkAsPregnant(PregnantWoman pregnantWoman)
        {
            if (Gender == Gender.Male)
                throw new DomainException("A Pregnant Woman can only be assigned to a female records!");

            if (PregnantWoman != null)
                throw new DomainException("This record is already linked to a Vaccinated Individual record!");

            PregnantWoman = pregnantWoman;
        }

        public void MarkAsVaccinatedIndvidual(VaccinatedIndividual vaccinatedIndividual)
        {
            if (VaccinatedIndividual != null)
                throw new DomainException("This record is already linked to a Vaccinated Individual record!");

            VaccinatedIndividual = vaccinatedIndividual;
        }
        public void AssignUser(User user)
        {
            if (User is not null)
                throw new DomainException("This record is already linked to a Vaccinated Individual record!");

            User = user;
        }
        public void ChangeName(string firstName, string secondName, string? thirdName, string lastName)
        {
            this.FirstName = firstName;
            this.SecondName = secondName;
            this.ThirdName = thirdName;
            this.LastName = lastName;
        }
        public void ChangeContactInfo(string phoneNumber)
        {
            this.PhoneNumber = phoneNumber;
        }
        public void UpdatePersonInfo(string firstName, string secondName, string? thirdName, string lastName, Gender gender, DateOnly dateOfBirth)
        {
            FirstName = firstName;
            SecondName = secondName;
            ThirdName = thirdName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Gender = gender;
        }
        public void MarkAsDeleted()
        {
            if (IsDeleted)
                return;

            IsDeleted = true;
        }

    }
}
