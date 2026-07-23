---
name: add-entity
description: Add a new CRUD entity to AEspejo.Clinic across all layers (Domain → EF config → migration → DTOs → validator → Mapster → service → DI → controller → frontend registry → i18n). Use when the user asks to add a new manageable entity/table/resource to the clinic system.
---

# Add a CRUD entity

This project drives almost all CRUD through generics — **do not invent a new controller/service/page pattern.**
Wire the new entity into the existing generics. Touch these layers in order.

## Before you start

- Two hard rules apply throughout: **comments in English**, and **no hardcoded user-visible strings**
  (frontend text is always an i18n key in `en.json` + `es.json`).
- Decide: is the entity **soft-deletable** (clinical/reference data that should never be physically
  deleted)? If yes it implements `ISoftDeletable` and uses the soft-delete service/repository.

## Steps

1. **Domain entity** — `src/AEspejo.Clinic.Domain/Entities/<Name>.cs`.
   Inherit `BaseEntity` (Guid id + audit fields). Add `ISoftDeletable` (`bool IsActive`) if soft-deletable.
   XML `/// <summary>` doc in English.

2. **EF configuration** — add an `IEntityTypeConfiguration<>` in the matching file under
   `src/AEspejo.Clinic.Infrastructure/Persistence/Configurations/`
   (`Clinical*`, `Finance*`, `Organization*`, or `Patient*`). Configure keys, lengths, relationships.
   Add the `DbSet<>` to `AppDbContext` and to the `IAppDbContext` interface if services need it.

3. **Migration** — `dotnet ef migrations add Add<Name> --project src/AEspejo.Clinic.Infrastructure`.
   (This targets the tenant model via its design-time factory; the migration is applied to every tenant DB.)

4. **DTOs** — in the matching `src/AEspejo.Clinic.Application/Dtos/*.cs` file, add
   `Create<Name>Dto`, `Update<Name>Dto`, `<Name>Dto`. Keep property names aligned with Mapster's
   `{NavProp}{Prop}` flattening (e.g. expose `BranchName` to auto-map `Branch.Name`).

5. **Validator** (optional) — add an `AbstractValidator<Create<Name>Dto>` (and update) in
   `src/AEspejo.Clinic.Application/Validation/Validators.cs`. If no rules are needed, the `NoOpValidator<>`
   fallback covers it.

6. **Mapster** — only if the DTO uses short names the flattening convention doesn't cover
   (e.g. `FirstName` instead of `UserFirstName`): add an explicit map in
   `src/AEspejo.Clinic.Application/Common/MapsterConfig.cs`.

7. **Service** — reuse a base:
   - `SoftDeleteCrudServiceBase<...>` for `ISoftDeletable` entities, else `CrudServiceBase<...>`.
   - If it needs no custom logic (search/includes), add it in
     `src/AEspejo.Clinic.Application/Services/SimpleCrudServices.cs`; otherwise its own file.
   - Override `BaseQuery` (Includes), `ApplySearch`, or `ApplyListFilter` as needed.

8. **DI registration** — register the service in `src/AEspejo.Clinic.Application/DependencyInjection.cs`:
   `services.AddScoped<ICrudService<Create<Name>Dto, Update<Name>Dto, <Name>Dto>, <Name>Service>();`

9. **Controller** — add a subclass in `src/AEspejo.Clinic.API/Controllers/EntityControllers.cs`:
   ```csharp
   [Route("api/<kebab-plural>")]
   public class <Name>sController(ICrudService<Create<Name>Dto, Update<Name>Dto, <Name>Dto> s)
       : CrudControllerBase<Create<Name>Dto, Update<Name>Dto, <Name>Dto>(s);
   ```
   Add `[Authorize(Roles = "Admin")]` if the resource is admin-only.

10. **Frontend registry** — append an `EntityConfig` to
    `src/AEspejo.Clinic.Client/src/features/registry.ts` (`key`, `resource`, `navKey`, `group`,
    `columns`, `fields`, `defaults`; use `reference: { resource, label }` for FK fields).
    Add the `nav.<key>` and every `fields.<prop>` key to **both** `en.json` and `es.json`.
    See `src/AEspejo.Clinic.Client/CLAUDE.md` for the `EntityConfig` shape.

11. **Regenerate & verify** — with the API running: `npm run gen:api` (updates `schema.d.ts`),
    then `dotnet build` (0 errors) and `npm run lint`. Confirm the new page loads and switches
    languages (en/es) with no hardcoded text.

## Reference examples

- Simple soft-delete entity end-to-end: `Room` (Domain `Room.cs` → `RoomService` in `SimpleCrudServices.cs`
  → `RoomsController` → `rooms` in `registry.ts`).
- Custom-logic service: `AppointmentService` (overlap validation), `PaymentService` (recalculates invoice).
- Shared-PK 1:1: `ProfessionalService` (Id = UserId).
