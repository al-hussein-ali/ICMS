using ICMS.Domain.Enums;
using ICMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Entites;

public class PregnantWoman : BaseEntity<int>
{
    private readonly List<PregnancyDetails> _pregnancyDetails = new();
    public IReadOnlyList<PregnancyDetails> PregnancyDetails => _pregnancyDetails.AsReadOnly();
    public string CurrentAddress { get; private set; } = string.Empty;
    public string AgeRange { get; private set; } = string.Empty;
    public byte PregnancyCount { get; private set; }
    public BloodGroup BloodGroup { get; private set; }
    public RhFactor RhFactor { get; private set; }
    public int PersonId { get; private set; }
    public int? UserId { get; private set; }

    public Person? Person { get; private set; }

    public User? User { get; private set; }

    private PregnantWoman()
    {
    }

    public static PregnantWoman Create(string currentAddress, string ageRange, byte pregnancyCount, BloodGroup bloodGroup, RhFactor rhFactor, int personId, int? userId = null)
    {
        if (string.IsNullOrWhiteSpace(currentAddress)) throw new DomainException("Current address is required");
        if (string.IsNullOrWhiteSpace(ageRange)) throw new DomainException("Age range is required");
        if (personId <= 0) throw new DomainException("Invalid person id");

        return new PregnantWoman
        {
            CurrentAddress = currentAddress,
            AgeRange = ageRange,
            PregnancyCount = pregnancyCount,
            BloodGroup = bloodGroup,
            RhFactor = rhFactor,
            PersonId = personId,
            UserId = userId
        };
    }

    public void AssignPerson(Person person)
    {
        if (person == null) throw new DomainException("Person is required");
        if (Person != null) throw new DomainException("Person already assigned");
        if (person.Id != 0 && person.Id != PersonId) throw new DomainException("Person id mismatch");

        Person = person;
        PersonId = person.Id;
    }

    public void AssignUser(User user)
    {
        if (user == null) throw new DomainException("User is required");
        if (User != null) throw new DomainException("User already assigned");
        if (user.Id != 0 && user.Id != UserId) throw new DomainException("User id mismatch");

        User = user;
        UserId = user.Id;
    }

    public void AddPregnancyDetails(PregnancyDetails pd)
    {
        if (pd == null) throw new DomainException("Pregnancy details required");
        if (_pregnancyDetails.Any(x => x.Id == pd.Id)) throw new DomainException("Pregnancy details already added");

        _pregnancyDetails.Add(pd);
        PregnancyCount = (byte)_pregnancyDetails.Count;
    }
}

