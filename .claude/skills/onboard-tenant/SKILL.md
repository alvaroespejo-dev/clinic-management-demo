---
name: onboard-tenant
description: Onboard a new tenant (client clinic) in AEspejo.Clinic — insert the Tenant row in the master DB and provision its dedicated database (schema + admin user + branch). Use when the user wants to add/create a new company/tenant/clinic to the system.
---

# Onboard a new tenant

A tenant = one client company with its **own database**, registered in the master DB. Mirror the demo
tenant in `API/Seeding/DevSeeder.cs`.

## Steps

1. **Create the `Tenant` row** in the master DB (`AEspejo.Clinic.Master.Entities.Tenant`):
   - `CompanyName` — display name.
   - `Subdomain` — **unique**, lowercase (e.g. `smileclinic`) → resolved from `smileclinic.<host>` or the
     `X-Tenant` header.
   - `DatabaseName` — e.g. `AEspejo_Clinic_SmileClinic`.
   - `ConnectionString` — derive from the master connection by swapping the catalog to `DatabaseName`
     (see `DevSeeder.BuildTenantConnection` using `SqlConnectionStringBuilder { InitialCatalog = ... }`).
   - `Plan` (`TenantPlan`), `DefaultLanguage` (ISO 639-1, e.g. `"es"`/`"en"` — must exist in the `Language`
     catalog), `ContactEmail`, `IsActive = true`, `CreatedAt = DateTimeOffset.UtcNow`.

2. **Provision its database** — call
   `TenantProvisioningService.ProvisionAsync(tenant, adminEmail, adminPassword)`. This creates/migrates
   the tenant DB (applies the `AppDbContext` migrations) and seeds an admin user + a branch, and
   initializes the org config with the company name.

3. **Verify** — reach the tenant via its subdomain or the `X-Tenant: <subdomain>` header and log in with
   the admin credentials you passed.

## Notes

- There is **no self-service onboarding endpoint yet** — this runs through a seeder/admin utility that
  resolves `MasterDbContext` + `TenantProvisioningService` from DI (same pattern as `DevSeeder.SeedAsync`).
  If the user wants self-service, build an admin-only endpoint (`custom-endpoint` skill) that performs
  steps 1–2 inside a scope; keep it `[Authorize(Roles = "Admin")]` and never expose it per-tenant.
- Do not put a raw password in the master DB; only the admin **password hash** lives in the tenant DB
  (handled by provisioning via `PasswordHasher`).

## Reference

`API/Seeding/DevSeeder.cs`, `Infrastructure/MultiTenancy/TenantProvisioningService.cs`,
`Master/Entities/Tenant.cs`, `Master/Entities/Language.cs`.
