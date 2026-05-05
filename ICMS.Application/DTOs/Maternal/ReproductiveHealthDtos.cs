using System;
using System.Collections.Generic;
using ICMS.Domain.Enums;

namespace ICMS.Application.DTOs.Maternal
{
    public record PreviousPregnancyComplicationsDto(
        bool VaginalBleeding,
        bool RecurrentMiscarriage,
        bool Diabetes,
        bool Epilepsy,
        bool HeartDisease,
        bool Preeclampsia,
        bool PretermBirth);

    public record PreviousPregnancyDeliveryComplicationsDto(
        bool CesareanSection,
        bool AssistedDelivery,
        bool StillbirthOrMultipleDeaths);

    public record PreviousPostpartumComplicationsDto(
        bool VaginalBleeding,
        bool PlacentaRetention,
        bool VaginalFistula,
        bool PuerperalSepsis,
        bool NeonatalDeath);

    public record StartPregnancyDto(
        int PersonId,
        DateOnly LMP,
        DateOnly EDD,
        PreviousPregnancyComplicationsDto? PreviousPregnancyComplications,
        PreviousPregnancyDeliveryComplicationsDto? PreviousPregnancyDeliveryComplications,
        PreviousPostpartumComplicationsDto? PreviousPostpartumComplications
    );

    public record UpdatePregnancyDto(
        DateOnly LMP,
        DateOnly EDD,
        PregnancyType PregnancyType
    );

    public record AddAncVisitDto(
        DateOnly VisitDate,
        int PregnancyDurationInWeeks,
        decimal WeightInKilo,
        string BloodPressure,
        int? TetanusDoseId,
        DateOnly? DoctorSuggestedNextVisit = null,
        string AppInUrineTest = "N/A",
        string OgttInUrineTest = "N/A",
        string FetalHeartbeat = "N/A",
        string? FetalHeartbeatValue = null,
        string FetalMovement = "N/A",
        string FetalPosition = "N/A",
        string AnaemiaOrHemoglobinType = "N/A",
        int? Id = null,
        string? ClinicalExaminationAndObservation = null,
        string? TreatmentsGiven = null,
        bool LegsSwelling = false,
        bool VaginalBleeding = false
    );

    public record NewbornDto(
        NewbornStatus Status,
        decimal Weight,
        Gender Gender
    );

    public record ConcludePregnancyDto(
        DateOnly DeliveryDate,
        BirthNature BirthNature,
        BirthLocationType BirthLocationType,
        string BirthLocationDetails,
        string IntrapartumComplications,
        string PostpartumComplications,
        List<NewbornDto> Newborns
    );

    public record PregnantWomanCreateDto(
        string AgeRange,
        byte PregnancyCount,
        BloodGroup BloodGroup,
        RhFactor RhFactor,
        int? UserId,
        Person.PersonCreateDto? PersonCreateDto,
        int? PersonId
    );

    public record PregnantWomanReadDto(
        int Id,
        string AgeRange,
        byte PregnancyCount,
        BloodGroup BloodGroup,
        RhFactor RhFactor,
        int PersonId,
        int? UserId,
        string? FirstName = null,
        string? SecondName = null,
        string? ThirdName = null,
        string? LastName = null,
        string? Gender = null
    );

    public record PregnantWomanDetailsDto(
        int Id,
        string AgeRange,
        byte PregnancyCount,
        BloodGroup BloodGroup,
        RhFactor RhFactor,
        int PersonId,
        int? UserId,
        ICMS.Application.DTOs.Person.PersonReadDto? Person,
        List<PregnancyDetailsReadDto> Pregnancies
    );

    public record PregnancyDetailsReadDto(
        int Id,
        DateOnly LastMenstrualPeriodDate,
        DateOnly ExpectedDeliveryDate,
        DateOnly? DeliveryDate,
        byte VisitsCount,
        byte NewbornCount,
        bool IsPregnancyDone,
        PregnancyType PregnancyType,
        BirthNature? BirthNature,
        BirthLocationType? BirthLocationType,
        string? BirthLocationDetails,
        string? IntrapartumComplications,
        string? PostpartumComplications,
        PreviousPregnancyComplicationsDto? PreviousPregnancyComplications,
        PreviousPregnancyDeliveryComplicationsDto? PreviousPregnancyDeliveryComplications,
        PreviousPostpartumComplicationsDto? PreviousPostpartumComplications,
        List<AddAncVisitDto> Visits,
        List<NewbornDto> Newborns
    );
}
