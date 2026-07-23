---
name: add-enum
description: Add a new enum to AEspejo.Clinic across backend and frontend. Enums serialize as strings over the API and each needs an i18n group in both locale files. Use when the user adds a new status/type/category enum (e.g. a new appointment status, payment method, tooth status).
---

# Add an enum

Enums are serialized as **strings** over the API (see `Program.cs` `JsonStringEnumConverter`), and the
frontend renders them as i18n keys (`t(\`<EnumName>.<Value>\`)`). Touch every layer below.

## Steps

1. **Domain** — add the enum to `src/AEspejo.Clinic.Domain/Enums/Enums.cs`.
   Use **explicit int values** like the existing ones (e.g. `Cash = 1, Card = 2, ...`). Never reorder or
   renumber existing members — the int is what EF stores.

2. **Use it** — reference the enum on the entity/DTO field. If a new entity field, an EF migration is
   needed (see the `ef-migration` skill).

3. **Frontend type** — add a string union in `src/AEspejo.Clinic.Client/src/lib/api/types.ts`
   matching the member names exactly (e.g. `export type PaymentMethod = 'Cash' | 'Card' | 'Transfer' | 'Insurance'`).

4. **i18n (both locales)** — add a group to **both**
   `src/lib/i18n/locales/en.json` **and** `es.json`:
   ```json
   "<EnumName>": { "Value1": "…", "Value2": "…" }
   ```
   Every member must have an entry in both files.

5. **Registry** — where the enum appears in a list/form, set it in
   `src/AEspejo.Clinic.Client/src/features/registry.ts`:
   - column: `{ key: '<field>', kind: 'enum', enumName: '<EnumName>' }`
   - field: `{ name: '<field>', kind: 'enum', enumName: '<EnumName>' }`

6. **Regenerate & verify** — with the API running: `npm run gen:api` (refreshes `schema.d.ts`), then
   `dotnet build` and confirm the value renders correctly in both languages.

## Reference

- Existing enums: `Domain/Enums/Enums.cs` (13 enums).
- String rendering: `formatCell` in `components/shared/CrudPage.tsx` (`case 'enum'`).
- Existing i18n groups: `UserRole`, `Gender`, `ToothStatus`, `PaymentMethod`, ... in the locale files.
