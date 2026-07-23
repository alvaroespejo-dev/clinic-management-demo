# <Feature name>

> Copy this file to `docs/requirements/<feature-slug>.md` and fill it in before implementation.
> Delete the hint lines (`>`) as you go.

## Goal & value
> One or two sentences: what problem this solves and who benefits.

## Scope / user stories
> As a <role>, I want <action>, so that <outcome>. List the concrete stories in scope.

## Business rules
> Constraints, calculations, edge cases, states/transitions. Be explicit — these become validators/tests.

## Data model impact
> New/changed entities, fields, relationships. Soft-delete? New enum? Master DB or tenant DB?
> Migration required?

## API / endpoints
> New or changed endpoints. Can the generic `CrudControllerBase` cover it, or is custom logic needed?
> DTO shape (Create/Update/Response). Auth roles required.

## UI / i18n
> Screens/components affected. New `registry.ts` entity? New i18n keys (list them — remember en + es).
> Any non-CRUD interaction.

## Multi-tenancy & permissions
> Per-tenant behavior, defaults for new tenants, role restrictions (`[Authorize(Roles=...)]` / `adminOnly`).

## Acceptance criteria
> Checklist a reviewer can verify. Include the language toggle (en/es) and "no hardcoded UI text".
- [ ]
- [ ]

## Out of scope
> Explicitly what this feature does NOT include, to prevent scope creep.
