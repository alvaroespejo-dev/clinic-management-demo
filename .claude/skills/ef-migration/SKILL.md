---
name: ef-migration
description: Create and apply EF Core migrations in AEspejo.Clinic. The app has TWO DbContexts — AppDbContext (tenant clinical model, applied to every tenant DB) and MasterDbContext (tenant/language catalog). Use when the user changes the data model, adds/removes a field or entity, or asks to add/run a migration.
---

# EF Core migrations

This solution has **two** contexts, each with its own design-time factory so `dotnet ef` works without
booting the API. Pick the right one.

| Context | Project | Factory | Holds |
|---|---|---|---|
| `AppDbContext` | `src/AEspejo.Clinic.Infrastructure` | `AppDbContextFactory` | Tenant clinical model (one DB **per company**) |
| `MasterDbContext` | `src/AEspejo.Clinic.Master` | `MasterDbContextFactory` | Tenant catalog + languages (single master DB) |

## Create a migration

Tenant model (most common — you changed an entity / EF configuration):
```
dotnet ef migrations add <Name> --project src/AEspejo.Clinic.Infrastructure
```
Master DB (you changed `Tenant` or `Language`):
```
dotnet ef migrations add <Name> --project src/AEspejo.Clinic.Master
```
Migrations land in the project's `Migrations/` folder together with the updated model snapshot.

## Apply migrations

Migrations are applied by `MigrateAsync`, not by you running `database update` manually:
- **Master DB**: applied by `DevSeeder` on startup (`master.Database.MigrateAsync()`).
- **Every tenant DB**: applied by `TenantProvisioningService.ProvisionAsync` (`ctx.Database.MigrateAsync`).
  In dev, running the API (`dotnet run --project src/AEspejo.Clinic.API`) triggers `DevSeeder`, which
  provisions/migrates the `demo` tenant.
- **Production**: a tenant migration must be rolled out to **each** company database — provisioning runs
  per tenant. Never assume migrating one DB is enough.

## Rules & gotchas

- Comments in English (XML docs on any new entity/config).
- Do **not** hand-edit a migration that has already been applied; add a new one instead.
- Do not delete the model snapshot (`*ModelSnapshot.cs`) — EF needs it to diff.
- After adding a migration, run `dotnet build` (0 errors) and, if a DTO shape changed, `npm run gen:api`.
- Don't leave throwaway/test migrations committed.

## Reference

- Design-time factories: `Infrastructure/Persistence/AppDbContextFactory.cs`, `Master/MasterDbContextFactory.cs`.
- Apply points: `API/Seeding/DevSeeder.cs`, `Infrastructure/MultiTenancy/TenantProvisioningService.cs`.
