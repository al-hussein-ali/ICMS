using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Enums;
using ICMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICMS.Domain.Entites.Maternal;

public class PregnantWoman : BaseEntity<int>
{
    private readonly List<PregnancyDetails> _pregnancyDetails = new();
    public IReadOnlyList<PregnancyDetails> PregnancyDetails => _pregnancyDetails.AsReadOnly();

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

    public static PregnantWoman Create(string ageRange, byte pregnancyCount, BloodGroup bloodGroup, RhFactor rhFactor,
        int personId, int? userId = null)
    {
        if (string.IsNullOrWhiteSpace(ageRange)) throw new DomainException("Age range is required");
        if (personId <= 0) throw new DomainException("Invalid person id");

        return new PregnantWoman
        {
            AgeRange = ageRange,
            PregnancyCount = pregnancyCount,
            BloodGroup = bloodGroup,
            RhFactor = rhFactor,
            PersonId = personId,
            UserId = userId
        };
    }

    public void Update(string ageRange, byte pregnancyCount, BloodGroup bloodGroup, RhFactor rhFactor,
        int? userId = null)
    {
        if (string.IsNullOrWhiteSpace(ageRange)) throw new DomainException("Age range is required");

        AgeRange = ageRange;
        PregnancyCount = pregnancyCount;
        BloodGroup = bloodGroup;
        RhFactor = rhFactor;

        if (userId.HasValue && userId.Value > 0)
        {
            UserId = userId.Value;
        }
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

        User = user;
        UserId = user.Id;
    }

    public void AddPregnancyDetails(PregnancyDetails pd)
    {
        if (pd == null) throw new DomainException("Pregnancy details required");
        if (_pregnancyDetails.Any(x => x.Id == pd.Id)) throw new DomainException("Pregnancy details already added");

        if (_pregnancyDetails.Any(p => !p.IsPregnancyDone))
        {
            throw new DomainException("Cannot add pregnancy details while another pregnancy is still active.");
        }

        _pregnancyDetails.Add(pd);
        PregnancyCount = (byte)_pregnancyDetails.Count;
    }

    public PregnancyDetails StartNewPregnancy(
        DateOnly lmp,
        DateOnly edd,
        PreviousPregnancyComplications? prevPreg,
        PreviousPregnancyDeliveryComplications? prevDeliv,
        PreviousPostpartumComplications? prevPost)
    {
        if (_pregnancyDetails.Any(p => !p.IsPregnancyDone))
        {
            throw new DomainException("Cannot start a new pregnancy while another one is still active.");
        }

        if (PregnancyCount > 0)
        {
            // If previous complications are not provided, we assume "No Complications" (all false)
            prevPreg ??=
                PreviousPregnancyComplications.CreateForNewPregnancy(false, false, false, false, false, false, false);
            prevDeliv ??= PreviousPregnancyDeliveryComplications.CreateForNewPregnancy(false, false, false);
            prevPost ??= PreviousPostpartumComplications.CreateForNewPregnancy(false, false, false, false, false);
        }
        else
        {
            // For the first pregnancy in the system, no previous complications should be recorded
            prevPreg = null;
            prevDeliv = null;
            prevPost = null;
        }

        var newPregnancy = ICMS.Domain.Entites.Maternal.PregnancyDetails.CreateForNewPregnancy(lmp, edd, this.Id);

        if (prevPreg != null) newPregnancy.AssignPreviousPregnancyComplications(prevPreg);
        if (prevDeliv != null) newPregnancy.AssignPreviousPregnancyDeliveryComplications(prevDeliv);
        if (prevPost != null) newPregnancy.AssignPreviousPostpartumComplications(prevPost);

        _pregnancyDetails.Add(newPregnancy);
        PregnancyCount++;

        return newPregnancy;
    }
}

