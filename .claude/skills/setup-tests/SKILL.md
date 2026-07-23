---
name: setup-tests
description: Bootstrap or extend the automated test suite for AEspejo.Clinic. The suite lives in tests/AEspejo.Clinic.Application.Tests (xUnit + EF Core InMemory) and covers Application services. Use when the user wants to set up testing, add a test project, or run the tests.
---

# Test suite setup

The project has a unit-test project for the Application layer:
`tests/AEspejo.Clinic.Application.Tests/` — **xUnit** + **EF Core InMemory**, one isolated in-memory
database per test. To write tests, use the `add-tests` skill; this skill covers the harness itself.

## What exists

- `AEspejo.Clinic.Application.Tests.csproj` — net10.0, references Domain + Application + Infrastructure;
  packages `Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio`,
  `Microsoft.EntityFrameworkCore.InMemory` (10.0.9, matching EF Core).
- `TestHelpers.cs`:
  - `TestDb.NewContext()` — builds an in-memory `AppDbContext`. **Passes only `DbContextOptions`
    (resolver = null)** so `OnConfiguring` is a no-op and no SQL Server connection is attempted.
  - `TestDb.RepositoryFor<TEntity>(ctx)` — the real `Repository<TEntity>` over that context.
  - `FakeCurrentUser` — `ICurrentUserService` returning a fixed `UserId`.
  - Static ctor calls `MapsterConfig.RegisterMappings()` once so `.Adapt<>()` behaves like production.

## Run

```
dotnet test tests/AEspejo.Clinic.Application.Tests/AEspejo.Clinic.Application.Tests.csproj
```

## Notes & constraints

- Use **InMemory**, not SQLite: the domain uses `DateTimeOffset` comparisons (appointment overlap) that
  SQLite handles poorly. InMemory supports the enum `HasConversion<int>()` mappings used in the model.
- The `AuditInterceptor` is **not** wired in tests (we construct the context manually), so `AuditLog`
  rows and auto timestamps are not written — assert on business state, not audit side effects.
- To add another test project (e.g. API integration), mirror the csproj and add it with
  `dotnet sln AEspejo.Clinic.sln add <path>`.
