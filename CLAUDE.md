# AEspejo.Clinic

Multi-tenant SaaS for dental clinics. Each client company (tenant) gets its own database;
a shared master database holds the tenant catalog and the supported-languages catalog.

**Stack:** .NET 10 · EF Core · SQL Server · React + Vite + TypeScript (`AEspejo.Clinic.Client`).

## Hard rules (do not break)

1. **All source-code comments must be in English** — backend and frontend. No Spanish comments.
2. **No user-visible string may be a literal.** Every label, button, toast, dialog, placeholder and
   UI validation message goes through i18n: add the key to **both** `src/AEspejo.Clinic.Client/src/lib/i18n/locales/en.json`
   **and** `es.json`, and render it with `t('...')`. Never hardcode English (or Spanish) text in `.tsx`.
3. **Reuse the generic CRUD pattern** (below) before writing a bespoke controller/service/page.

## Architecture (layers & dependency direction)

`Domain` ← `Application` ← `Infrastructure` / `API`. The `Master` project (tenant + language catalog)
is independent of the tenant clinical model. Frontend lives in `AEspejo.Clinic.Client`.

- `AEspejo.Clinic.Domain` — entities (`BaseEntity`, `ISoftDeletable`), enums. No dependencies.
- `AEspejo.Clinic.Application` — DTOs, services, `ICrudService<,,>`, validators, Mapster config, `Result<T>`.
- `AEspejo.Clinic.Infrastructure` — EF Core (`AppDbContext`), repositories, `AuditInterceptor`, multi-tenancy resolvers.
- `AEspejo.Clinic.Master` — `MasterDbContext`: tenants + languages, always on the master DB.
- `AEspejo.Clinic.API` — controllers, `TenantResolutionMiddleware`, JWT auth, DI composition, `DevSeeder`.
- `AEspejo.Clinic.Client` — React SPA (see its own `CLAUDE.md`).

## Multi-tenancy

- **Master DB** = catalog of tenants (each row points to that company's own connection string) + languages.
- **Tenant DB** = the full clinical model; one database per company.
- A request's tenant is resolved by `TenantResolutionMiddleware` from the **subdomain**
  (e.g. `demo.localhost`) or the **`X-Tenant` header**; without a valid tenant → 400.
- Two `DbContext`s (`MasterDbContext`, `AppDbContext`), each with a design-time factory so
  `dotnet ef` can create migrations without booting the API.
- A tenant migration is applied to **every** company database, not just one.

## Database providers (SQL Server / SQLite)

The app runs on **SQL Server** (production) or **SQLite** (a cheap, dependency-free demo — e.g. a portfolio
deploy on Azure App Service). The engine is chosen by config **`Database:Provider`** = `SqlServer` (default)
| `Sqlite`, and applies to both the master and every tenant database.

- One place knows the concrete providers: `Infrastructure/Persistence/DatabaseProvider.cs`
  (`DatabaseProviderExtensions`) — `UseConfiguredDatabase(...)`, `EnsureDatabaseAsync(...)`,
  `BuildSqliteConnectionString(...)`. Everything else selects via the `DatabaseProvider` enum.
- Schema strategy: SQL Server → `MigrateAsync()` (the versioned migrations); SQLite → `EnsureCreatedAsync()`
  (schema built from the current model; **no SQLite migrations are maintained**).
- SQLite = **one `.db` file per tenant** under the data dir (`%HOME%\data` on Azure — persistent — else `./data`).
- **DateTimeOffset gotcha:** SQLite can't `ORDER BY`/compare `DateTimeOffset`. Both contexts apply a
  UTC-ticks value converter in `OnModelCreating` guarded by `Database.IsSqlite()`; SQL Server keeps native
  `datetimeoffset`. Any new `DateTimeOffset` column is covered automatically (the converter scans all props).
- Seeding runs in Development **or** when `Seed:Enabled=true` (so the non-Development Azure demo still seeds).
- Azure demo App Settings: `Database__Provider=Sqlite`, `ConnectionStrings__MasterConnection=Data Source=D:\home\data\master.db`, `Seed__Enabled=true`.
- Demo data: `scripts/seed-demo-month.sql` (T-SQL / SQL Server) and `scripts/seed-demo-month.sqlite.sql`
  (SQLite port) both seed ~1 month of activity. The SQLite one honours EF's storage formats
  (Guid = UPPERCASE TEXT, DateTimeOffset = UTC-ticks INTEGER, DateOnly = 'yyyy-MM-dd' TEXT, decimal = TEXT).

## Generic CRUD pattern (backend)

Adding an entity almost never needs new infrastructure — wire into the existing generics:

- Controller: subclass `CrudControllerBase<TCreate,TUpdate,TResponse>` in
  `API/Controllers/EntityControllers.cs` with `[Route("api/...")]` (harden with `[Authorize(Roles=...)]` if needed).
- Service: reuse `CrudServiceBase` (physical delete) or `SoftDeleteCrudServiceBase` (soft delete);
  simple ones live in `Application/Services/SimpleCrudServices.cs`. Register in `Application/DependencyInjection.cs`.
- Mapping: **Mapster** flattens `{NavProp}{Prop}` automatically (e.g. `Branch.Name` → `BranchName`).
  When the DTO uses a short name (e.g. `FirstName` from `User.FirstName`), add an explicit map in
  `Application/Common/MapsterConfig.cs`.
- Validation: **FluentValidation** in `Application/Validation/Validators.cs`; a `NoOpValidator<>` is the fallback.
- Soft-delete: entities implementing `ISoftDeletable` are deleted via `SoftDeleteRepository`
  (sets `IsActive = false`); the closed DI registration wins over the open generic.
- Audit: `AuditInterceptor` writes an `AuditLog` row on every Created/Updated/Deleted automatically.
  `AuditLog` is **read-only** through the API.

The frontend mirror (`createCrud()` + `CrudPage` driven by `features/registry.ts`) is documented in
`src/AEspejo.Clinic.Client/CLAUDE.md`.

## Commands

Backend (repo root):
- `dotnet build`
- `dotnet format AEspejo.Clinic.sln` — apply the code standard (`.editorconfig` + analyzers); `--verify-no-changes` to check
- `dotnet run --project src/AEspejo.Clinic.API` — API at **http://localhost:5287**
- `dotnet ef migrations add <Name> --project src/AEspejo.Clinic.Infrastructure` (tenant model, `AppDbContext`)
- `dotnet ef migrations add <Name> --project src/AEspejo.Clinic.Master` (master DB, `MasterDbContext`)

Frontend (`src/AEspejo.Clinic.Client`):
- `npm run dev` · `npm run build` · `npm run lint` (oxlint)
- `npm run gen:api` — regenerates `src/lib/api/schema.d.ts` from the OpenAPI doc at :5287 (**API must be running**)

## Available skills (`.claude/skills/`)

- **add-entity** — add a CRUD entity across all layers (Domain → EF → migration → DTO → service → DI → controller → registry → i18n).
- **add-enum** — add an enum end-to-end (Domain enum → frontend string union → i18n group in both locales).
- **ef-migration** — create/apply migrations for the two contexts (`AppDbContext` tenant, `MasterDbContext`).
- **custom-endpoint** — service business logic + controller action when generic CRUD isn't enough.
- **translatable-catalog** — per-language entity (`ServiceCatalog` + `*Translation`) pattern.
- **onboard-tenant** — register a `Tenant` row and provision its database.
- **i18n-audit** — scan for hardcoded UI text, en/es key parity, and non-English comments.
- **coding-standards** — apply/verify the .NET 10 standard (`.editorconfig` + analyzers, `dotnet format`).
- **pre-commit-review** — self-review checklist before finishing a change.
- **setup-tests** / **add-tests** — xUnit + EF Core InMemory suite for Application services.

## Feature requirements

Specs for new features go in `docs/requirements/` (copy `TEMPLATE.md` per feature). Read the relevant
spec before implementing.

## Dev seed

`DevSeeder` provisions a `demo` tenant with admin `admin@demo.local` / `Admin12345`.
