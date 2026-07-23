---
name: translatable-catalog
description: Add an entity whose visible text is translated per language in AEspejo.Clinic, following the ServiceCatalog + ServiceCatalogTranslation pattern (main row for non-translatable fields, one translation row per language). Use when a catalog/lookup needs localized name/description/category per language.
---

# Translatable catalog entity

Distinct from plain CRUD: the visible text (name, description, category) lives in a separate `*Translation`
entity, one row per language, resolved at read time. Mirror the existing `ServiceCatalog` implementation.

## Steps

1. **Main entity** — `Domain/Entities/<Name>.cs`: only **non-translatable** fields (code, price, flags,
   FKs) + a collection of translations.

2. **Translation entity** — `Domain/Entities/<Name>Translation.cs`: `<Name>Id`, `LanguageCode`
   (ISO 639-1), and the localized text fields.

3. **EF config** — configure a **unique index on `(<Name>Id, LanguageCode)`** and the
   one-to-many relationship. See `ServiceCatalogTranslation` config in the
   `Infrastructure/Persistence/Configurations/` files. Add a migration (`ef-migration` skill).

4. **DTOs** — response DTO exposes a **resolved** `Name`/`Description` (single language), plus the raw
   translations for create/update. See `Application/Dtos/CatalogAndClinicalDtos.cs`.

5. **Service** — resolve text to the requested language with fallback: **requested lang → tenant default
   lang → first available translation**. On update, **replace** the translation set. Copy the approach in
   `Application/Services/ServiceCatalogService.cs` (`MapToDto` + translation replacement).

6. **Controller** — resolve the language from `?lang=` query or `Accept-Language`, fallback `"es"`.
   See `API/Controllers/ServiceCatalogController.cs`. Return via `ApiResults` helpers.

7. **Frontend** — if listed/edited in the UI, add to `registry.ts` (may need a custom page if the
   translation editing is richer than the generic form). All UI text via i18n (en.json + es.json).

8. **Verify** — `dotnet build`, `npm run gen:api` if DTOs changed, and check name resolution with
   `?lang=en` vs `?lang=es`.

## Reference (copy this)

`ServiceCatalog` end-to-end: `Domain/Entities/ServiceCatalog.cs` + `ServiceCatalogTranslation.cs`,
`ServiceCatalogService.cs`, `ServiceCatalogController.cs`, and the languages catalog in
`AEspejo.Clinic.Master` (`Language`).
