---
name: feature-addition
description: Guide for adding new vertical slices (features) to the ICMS system following Clean Architecture.
---

# ICMS Feature Addition Guide

This guide describes the standard workflow for adding a new feature (entity and its associated logic) to the Integrated Case Management System (ICMS).

## Architectural Overview

ICMS follows Clean Architecture principles with four main layers:
1.  **Domain**: Core business logic and entities.
2.  **Application**: Use cases, DTOs, interfaces, and services.
3.  **Infrastructure**: Data persistence (EF Core), external services.
4.  **API**: REST endpoints.

---

## Step 1: Domain Layer

### 1.1 Add the Entity
-   **Path**: `ICMS.Domain/Entites/[Module]/[EntityName].cs`
-   **Base Class**: Must inherit from `BaseEntity<int>` (or the appropriate key type).
-   **Encapsulation**: 
    -   Use a `private` parameterless constructor for EF Core.
    -   Use a `public static [EntityName] Create(...)` factory method for instantiation.
    -   Use `private set` for properties to enforce invariants.
-   **Validation**: Throw `DomainException` for invalid state transitions.

### 1.2 Add Value Objects / Enums
-   Place value objects in `ICMS.Domain/ValueObjects`.
-   Place enums in `ICMS.Domain/Enums`.

---

## Step 2: Infrastructure Layer (Persistence)

### 2.1 Entity Configuration
-   **Path**: `ICMS.Infrastructure/Persistence/Config/[Module]/[EntityName]Config.cs`
-   **Logic**: Use `IEntityTypeConfiguration<[EntityName]>` to define table names, keys, and property constraints.

### 2.2 Repository
-   **Interface**: Define `I[EntityName]Repository` in `ICMS.Application/Interfaces/Repositories`.
-   **Implementation**: Add the implementation in `ICMS.Infrastructure/Repositories/[Module]/[EntityName]Repository.cs`.
-   **Naming**: Always use the full name (e.g., `IFeatureRepository`).

### 2.3 Unit of Work
-   Register the repository in `ICMS.Application/Interfaces/IUnitOfWork.cs`.
-   Implement the registration in `ICMS.Infrastructure/Repositories/UnitOfWork.cs`.

### 2.4 DB Context & Migration
-   Add a `DbSet<[EntityName]>` to `AppDbContext`.
-   Run `dotnet ef migrations add [MigrationName] --project ICMS.Infrastructure --startup-project ICMS.API`.
-   **Note**: If adding constraints to existing data, use `migrationBuilder.Sql()` to scrub data before adding foreign keys.

---

## Step 3: Application Layer

### 3.1 DTOs
-   **Path**: `ICMS.Application/DTOs/[Module]/`
-   **Naming**:
    -   `[EntityName]ReadDto`: For return values.
    -   `[EntityName]CreateDto`: For creation requests.
    -   `[EntityName]UpdateDto`: For update requests.
-   **Type**: Use C# `record` for immutability.

### 3.2 Mapping Extensions
-   **Path**: `ICMS.Application/Extensions/[EntityName]Extensions.cs`
-   **Logic**: Create `ToReadDto()` and `ToDetailsDto()` extension methods on the entity.

### 3.3 Service
-   **Interface**: Define `I[EntityName]Service` in `ICMS.Application/Interfaces/Services`.
-   **Implementation**: Add implementation in `ICMS.Application/Services/[EntityName]Service.cs`.
-   **Logic**: Coordinate between the Unit of Work and the Domain models.

### 3.4 Validations
-   **Path**: `ICMS.Application/Validators/[Module]/[EntityName]CreateDtoValidator.cs`
-   **Logic**: Use `FluentValidation`'s `AbstractValidator<T>`.

---

## Step 4: API Layer

### 4.1 Controller
-   **Path**: `ICMS.API/Controllers/[Module]Controller.cs`
-   **Routing**: Use `[Route("api/[controller]")]` or specialized routes like `api/v1/feature`.
-   **Action Methods**:
    -   `GET`: Return Paged results or single entity details.
    -   `POST`: Create new entity.
    -   `PUT/PATCH`: Update existing.
    -   `DELETE`: Remove entity.

---

## Naming & Style Rules
-   **Repositories**: Always use full name `...Repository`.
-   **Asynchronous**: All I/O operations must be `async` and accept a `CancellationToken`.
-   **Error Handling**: Let global exception handler catch specific Domain/NotFound exceptions.
