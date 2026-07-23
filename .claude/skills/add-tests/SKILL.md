---
name: add-tests
description: Write a unit test for an AEspejo.Clinic Application service using the existing xUnit + EF Core InMemory harness. Use when the user adds/changes service business logic (validation, overlap checks, recalculation, side effects) and wants test coverage.
---

# Add a service test

Tests live in `tests/AEspejo.Clinic.Application.Tests/`. Prerequisite harness: the `setup-tests` skill.
Focus coverage on **business logic**, not the generic CRUD base (which is already exercised indirectly).

## Pattern

Arrange an in-memory context, seed prerequisites, act through the service, assert on the `Result` and on
the persisted state:

```csharp
[Fact]
public async Task Does_the_thing()
{
    using var ctx = TestDb.NewContext();
    // seed prerequisites directly on ctx if the service reads them
    var service = new SomeService(ctx, TestDb.RepositoryFor<SomeEntity>(ctx), new FakeCurrentUser());

    var result = await service.CreateAsync(dto);

    Assert.Equal(ResultStatus.Ok, result.Status);   // or Conflict / Invalid / NotFound
    Assert.Equal(expected, result.Value!.SomeField);
    // and/or assert persisted state: Assert.Single(ctx.SomeEntities);
}
```

Rules:
- One `TestDb.NewContext()` per test → isolated database, no cross-test bleed.
- Assert on `result.Status` (`ResultStatus.Ok/Conflict/Invalid/NotFound/Unauthorized`) and `result.Value`.
- If the service stamps the current user, pass `new FakeCurrentUser(knownId)` and assert the id.
- Comments in English.

## What to cover first (highest value)

- Rules that return non-`Ok` results: overlaps, conflicts, missing references, validation.
- Recalculation / cross-entity side effects (e.g. invoice totals after a payment).
- Status transitions.

## Reference tests

- `AppointmentServiceTests` — overlap → `Conflict`; back-to-back allowed; cancelled ignored;
  current-user + status stamping.
- `PaymentServiceTests` — missing invoice → `Invalid`; partial → `PartiallyPaid`; full → `Paid`;
  delete recalculates back down.

Run: `dotnet test tests/AEspejo.Clinic.Application.Tests/AEspejo.Clinic.Application.Tests.csproj`.
