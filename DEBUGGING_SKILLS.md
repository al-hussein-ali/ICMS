# ICMS Backend Debugging Guide

This document provides a systematic approach to debugging errors in the ICMS backend, based on its Clean Architecture and data flow.

## 🧱 The Debugging Pipeline

When an error occurs, trace it through these layers in order:

### 1. ICMS.API (Entry & Global Handling)
- **Check `GlobalExceptionHandler.cs`**: This is the final catch-all. If you get a 500 error without a specific message, it's likely hitting the default case here.
- **FluentValidation**: If you get a 400 "ValidationError", the request DTO likely failed validation. Check the `Validators` in `ICMS.Application`.
- **Mapping**: We use `toPascalCase` on the frontend and `toCamelCase` on the response. If data is `null` in your Controller, check if the JSON property names match the DTO property names.

### 2. ICMS.Application (Business Logic)
- **Service Layer**: Most `DomainException`s are thrown here. Use breakpoints at the start of the service method.
- **IUnitOfWork**: If data isn't saving, verify `_unitOfWork.SaveChangesAsync()` is actually called and not bypassed by an early return or exception.
- **Dependency Injection**: If you get a "System.InvalidOperationException: Unable to resolve service...", check `ServiceCollectionExtensions.cs` in the relevant layer to ensure the interface is registered.

### 3. ICMS.Domain (Core Logic)
- **Domain Exceptions**: Look for `throw new DomainException("ErrorKey")`. These keys must exist in the frontend `locales` files to be user-friendly.
- **Entity State**: Check if entity factory methods (e.g., `VaccinatedIndividual.Create(...)`) are being called correctly.

### 4. ICMS.Infrastructure (Persistence)
- **SQL Profiling**: EF Core logs SQL to the console in `Development`. Watch the terminal to see the actual queries being executed.
- **Migrations**: If you get "Table not found" or "Column mismatch", ensure `dotnet ef database update` has been run.

---

## 🛠 Common Debugging Scenarios

### Scenario A: "The request body is empty in the Controller"
1. **Cause**: PascalCase mismatch or incorrect `[FromBody]` attribute.
2. **Fix**: Check the Network tab in the browser. Ensure the request payload is in `PascalCase` (our `apiClient.js` handles this, but verify it's not a `FormData` object which skips mapping).

### Scenario B: "Database changes aren't persisting"
1. **Cause**: `UnitOfWork.SaveChangesAsync()` wasn't called.
2. **Fix**: Ensure the service method completes without throwing and calls `await _unitOfWork.SaveChangesAsync()`.

### Scenario C: "401 Unauthorized but I am logged in"
1. **Cause**: Token expired or Audience/Issuer mismatch in `appsettings.json`.
2. **Fix**: Check `Program.cs` JWT configuration. Verify the `Expired` flag in the JWT payload using `jwt.io`.

---

## 📝 Best Practices for Debugging
- **Use `ILogger`**: Add `_logger.LogInformation("Processing {Id}", id);` to track flow.
- **Visual Studio Debugger**: Use "Step Into" (F11) to trace logic across layers.
- **Check `build_errors.txt`**: If the project fails to start, this file usually contains the compiler or DI container error.
- **Watch the Console**: ASP.NET Core outputs all runtime exceptions and EF Core queries to the terminal.

> [!TIP]
> Always check if the `DomainException` key you are throwing in the backend has a corresponding translation in `frontend/src/locales/en.json`.
