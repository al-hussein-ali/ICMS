---
name: VALIDATION_AND_LINK
description: Enforces strict frontend/backend API contract matching. The frontend MUST inspect backend DTOs, validators, and business rules before any implementation. Any mismatch is a fullstack architectural failure.
---

# RULE 1.1 — FRONTEND FIELDS MUST EXACTLY MATCH API CONTRACT REQUIREMENTS

## OBJECTIVE

Before ANY frontend form, request model, payload builder, or API call is implemented,
the developer or AI agent MUST inspect the backend endpoint contract carefully.

The frontend MUST verify that:
- every required backend field exists in the UI
- field names match exactly
- field types match exactly
- nullable behavior matches
- enum values match
- nested object structures match
- arrays match
- validation rules match
- required business fields are not omitted

The frontend MUST NEVER guess what the API expects.

The backend endpoint definition is the SOURCE OF TRUTH.

---

# REQUIRED ANALYSIS BEFORE IMPLEMENTATION

Before building a frontend form or API integration:

## ALWAYS INSPECT

- Controller endpoint
- Request DTO
- FluentValidation validator
- Service layer business rules
- Enum definitions
- Required relationships
- Nullable fields
- Database constraints if necessary

---

# REQUIRED FLOW

Frontend implementation MUST follow this order:

```
Backend Endpoint
→ DTO
→ Validator
→ Business Rules
→ Frontend Form
→ Frontend Validation
→ API Payload
```

NEVER reverse this flow.

---

# EXAMPLE ANALYSIS WORKFLOW

---

## BACKEND ENDPOINT

```csharp
[HttpPost]
public async Task<IActionResult> CreateVaccinatedIndividual(
    CreateVaccinatedIndividualDto dto)
```

---

## DTO

```csharp
public class CreateVaccinatedIndividualDto
{
    public string FullName { get; set; }

    public string PhoneNumber { get; set; }

    public DateTime BirthDate { get; set; }

    public int VaccineTypeId { get; set; }

    public bool IsPregnant { get; set; }
}
```

---

## VALIDATOR

```csharp
RuleFor(x => x.FullName)
    .NotEmpty()
    .MaximumLength(150);

RuleFor(x => x.PhoneNumber)
    .Matches(@"^[0-9]{9,15}$");

RuleFor(x => x.BirthDate)
    .LessThan(DateTime.UtcNow);

RuleFor(x => x.VaccineTypeId)
    .GreaterThan(0);
```

---

# REQUIRED FRONTEND IMPLEMENTATION

The frontend MUST contain matching fields:

```js
{
  fullName: "",
  phoneNumber: "",
  birthDate: "",
  vaccineTypeId: null,
  isPregnant: false
}
```

---

# REQUIRED VALIDATION

Frontend MUST enforce:

- required FullName
- max length 150
- numeric phone regex
- valid birth date
- vaccineTypeId > 0

---

# FORBIDDEN

## NEVER OMIT REQUIRED API FIELDS

BAD:

```js
{
  fullName: "",
  birthDate: ""
}
```

Missing:

- phoneNumber
- vaccineTypeId

This causes:

- failed requests
- inconsistent UX
- hidden validation failures

---

## NEVER ADD UNKNOWN FIELDS

BAD:

```js
{
  fullName: "",
  uiExpanded: true,
  selectedCard: 5
}
```

UI-only state MUST NEVER be sent to APIs.

---

## NEVER USE INCORRECT TYPES

BAD:

```js
vaccineTypeId: ""
```

when backend expects:

```csharp
int VaccineTypeId
```

This causes:

- parsing failures
- model binding issues
- inconsistent validation

---

# REQUIRED TYPE MAPPING RULES

| Backend Type | Frontend Type                   |
| ------------ | ------------------------------- |
| string       | string                          |
| int          | number                          |
| long         | number                          |
| decimal      | number                          |
| bool         | boolean                         |
| DateTime     | ISO date string                 |
| Guid         | string                          |
| enum         | controlled numeric/string value |
| List\<T\>    | array                           |
| object       | object                          |

---

# REQUIRED VALIDATION SYNCHRONIZATION

Frontend validation rules MUST mirror backend validators.

If backend changes:

- required fields
- regex
- ranges
- enums
- max lengths

Then frontend validation MUST also be updated immediately.

---

# CRITICAL RULE

ANY mismatch between:

- frontend fields
- frontend validation
- backend DTOs
- backend validators

is considered a **CONTRACT FAILURE**.

Contract failures can cause:

- rejected requests
- hidden bugs
- invalid healthcare data
- silent corruption
- broken workflows

---

# REQUIRED API CONTRACT CHECKLIST

Before approving ANY frontend feature, verify:

## FIELD MATCHING

- [ ] All DTO fields exist in frontend
- [ ] No unknown fields are sent
- [ ] Names match correctly
- [ ] Nested objects match

---

## TYPE MATCHING

- [ ] Numbers are numbers
- [ ] Dates are valid ISO dates
- [ ] Booleans are booleans
- [ ] Arrays match structure

---

## VALIDATION MATCHING

- [ ] Required fields match
- [ ] Regex rules match
- [ ] Range rules match
- [ ] Enum rules match
- [ ] Max lengths match

---

## BUSINESS RULE MATCHING

- [ ] Workflow order validated
- [ ] Cross-field logic validated
- [ ] Conditional fields handled properly

---

# REQUIRED AI AGENT BEHAVIOR

When implementing or auditing frontend/backend integration:

## ALWAYS

- inspect backend endpoint first
- inspect DTOs
- inspect validators
- inspect enums
- inspect service rules

THEN:

- build frontend fields
- build validation
- build payload mapping

---

## NEVER

- invent payload structures
- guess required fields
- assume field types
- hardcode enums blindly
- skip validator inspection

---

# FINAL OBJECTIVE

The frontend MUST behave as a **STRICT CLIENT** of the backend contract.

The backend defines:

- required fields
- allowed formats
- business rules
- accepted structures

The frontend MUST mirror these rules accurately.

Any frontend/backend mismatch is considered a **FULLSTACK ARCHITECTURAL FAILURE**.
