---
name: custom-endpoint
description: Add business logic beyond the generic CRUD in AEspejo.Clinic — a service method with validation/side effects and a controller action. Use when a feature needs custom rules (overlap checks, recalculation, cross-entity updates, extra actions) that CrudControllerBase/CrudServiceBase don't cover.
---

# Custom (non-CRUD) endpoint

Use this only when the generic CRUD (`CrudControllerBase` + `CrudServiceBase`) genuinely can't express
the behavior. Otherwise follow the `add-entity` skill.

## Steps

1. **Service logic** — in the entity's service (`Application/Services/*`):
   - Override `CreateAsync`/`UpdateAsync`/`DeleteAsync` to add rules or side effects, **or** add a new
     method. Always return `Result<T>` / `Result`.
   - Return typed failures: `Result<T>.Invalid("…")`, `.NotFound(...)`, `.Conflict("…")`,
     `.Unauthorized(...)` — these map to 400/404/409/401.
   - Get the current user via `ICurrentUserService` (e.g. stamp `CreatedByUserId`/`ReceivedByUserId`).
   - Cross-entity reads/writes go through `Db` (the exposed `IAppDbContext`) or another repository.
   - Validation stays in FluentValidation (`Validators.cs`) for shape; business rules live in the service.
   - Auditing is automatic (`AuditInterceptor` on `SaveChangesAsync`) — don't write `AuditLog` yourself.

2. **Controller action** — map the `Result` to HTTP with the extension methods in
   `API/Controllers/ApiResults.cs`:
   - `return result.ToActionResult();` (200/204 or the error status)
   - `return result.ToCreatedResult();` (201 on success)
   Add the action to the entity's controller in `EntityControllers.cs` (or a dedicated controller if it's
   not tied to one entity). Guard with `[Authorize(Roles = "...")]` as needed.

3. **Frontend** — if the UI calls it, add a typed call in `src/lib/api/` (reuse the `api` client) and any
   i18n keys for new messages (en.json + es.json). Run `npm run gen:api` if DTOs changed.

4. **Verify** — `dotnet build` (0 errors) and exercise the endpoint (success + each failure path).

## Reference examples

- `AppointmentService` — schedule-overlap validation returning `Conflict`.
- `PaymentService` — recalculates the invoice `PaidAmount`/`Status` after each payment change.
- `ToothRecordService` — stamps `UpdatedByUserId` from `ICurrentUserService`.
- `ServiceCatalogController` — action beyond plain CRUD (language resolution).
