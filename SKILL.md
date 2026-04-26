# ICMS Backend Development Skills & Rules

This document outlines the architectural principles, code quality standards, and project structure rules for the ICMS Backend.

## 🏗 Project Structure (Clean Architecture)

The project follows the **Clean Architecture** pattern, ensuring separation of concerns and high testability.

### 1. ICMS.Domain (Core)
- **Purpose**: Contains the heart of the application (Entities, Value Objects, Domain Exceptions, Enums, and Core logic).
- **Dependencies**: **NONE**. It must not depend on any other layer or external framework (except for essential .NET types).
- **Rules**:
  - Entities should have private setters to enforce encapsulation.
  - Use **Factory Methods** (e.g., `Create`) for entity instantiation.
  - Implement domain logic inside entities (Rich Domain Model) rather than just having properties.

### 2. ICMS.Application
- **Purpose**: Defines the "What" of the application. Contains Interfaces, DTOs, Application Services, and Validators.
- **Dependencies**: Depends ONLY on `ICMS.Domain`.
- **Rules**:
  - Use **DTOs** (Data Transfer Objects) for all API requests and responses. Never return Entities directly.
  - Business logic flows through **Services**.
  - Use **FluentValidation** for input validation.
  - All external dependencies (DB, Email, Logging) must be accessed via **Interfaces**.

### 3. ICMS.Infrastructure
- **Purpose**: Implementation of the "How". Contains Persistence (EF Core), Repositories, and External Service implementations.
- **Dependencies**: Depends on `ICMS.Application` and `ICMS.Domain`.
- **Rules**:
  - Implement the **Repository Pattern** and **Unit of Work**.
  - Keep infrastructure-specific logic (e.g., SQL queries, API clients) isolated here.

### 4. ICMS.API
- **Purpose**: Entry point for the application (Controllers, Middleware, DI Configuration).
- **Dependencies**: Depends on `ICMS.Application` and `ICMS.Infrastructure` (strictly for Dependency Injection setup).
- **Rules**:
  - Controllers should be "Thin" – they should only delegate work to Services.
  - Handle global exceptions via Middleware.

---

## 🧹 Clean Code Rules

### 1. Naming Conventions
- **Classes/Methods**: `PascalCase` (e.g., `VaccinatedIndividualService`, `GetAllAsync`).
- **Variables/Parameters**: `camelCase` (e.g., `unitOfWork`, `paginationParams`).
- **Interfaces**: Prefix with `I` (e.g., `IUnitOfWork`).
- **Async Methods**: Must end with the `Async` suffix.

### 2. SOLID Principles
- **S**ingle Responsibility: Each class should have one, and only one, reason to change.
- **O**pen/Closed: Software entities should be open for extension but closed for modification.
- **L**iskov Substitution: Subtypes must be substitutable for their base types.
- **I**nterface Segregation: Clients should not be forced to depend on methods they do not use.
- **D**ependency Inversion: Depend on abstractions, not concretions.

### 3. General Rules
- **DRY (Don't Repeat Yourself)**: Abstract common logic into Helpers or Extensions.
- **KISS (Keep It Simple, Stupid)**: Avoid over-engineering. Prefer readable code over "clever" code.
- **Small Methods**: Methods should ideally be under 20-30 lines. If it's longer, refactor into smaller private methods.
- **Async Everywhere**: Always use `Task` and `await` for I/O bound operations. Pass `CancellationToken` through the stack.

---

## 🛠 Project-Specific Patterns

### 1. Error Handling
- Use **Custom Exceptions** (e.g., `NotFoundException`, `DomainException`).
- Throw exceptions in the Service layer with localized keys.
- Let the Global Exception Middleware handle the conversion to HTTP responses.

### 2. Dependency Injection
- Use **Constructor Injection**.
- Prefer **Primary Constructors** (C# 12) for brevity.
- Register services in `ServiceCollectionExtensions` within their respective layers.

### 3. Data Mapping
- Use Manual Mapping or Extension Methods (e.g., `ToReadDto()`) for converting Entities to DTOs to maintain full control and performance.

### 4. Persistence
- Always use `IUnitOfWork` to commit changes.
- Avoid calling `SaveChangesAsync` multiple times in a single service method unless necessary.

---

> [!IMPORTANT]
> **Code Review Checklist:**
> - Does the code follow the dependency flow?
> - Are there any hardcoded strings that should be localized?
> - Is `CancellationToken` being passed?
> - Are DTOs used for all API boundaries?
