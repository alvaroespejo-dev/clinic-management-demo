---
name: pre-commit-review
description: Self-review checklist for AEspejo.Clinic before finishing or committing a change — conventions, i18n coverage, generic-CRUD reuse, build/lint, migrations. Use when wrapping up a change, before committing, or when the user asks to review the pending work.
---

# Pre-commit review

Run this checklist over the pending change (`git diff`) before declaring it done. Fix anything that fails.

## Checklist

1. **Comments in English** — no Spanish comments introduced. (Run the `i18n-audit` skill's comment check.)
2. **No hardcoded UI text** — every new user-visible string is a `t('...')` key present in **both**
   `en.json` and `es.json`. (Run the `i18n-audit` skill.)
3. **Generic CRUD reused** — a new entity uses `CrudControllerBase`/`CrudServiceBase` +
   `registry.ts`, not a bespoke controller/service/page. Custom logic only where justified
   (see `custom-endpoint`).
4. **DTO change → types regenerated** — if any DTO changed, `npm run gen:api` was run (API up on :5287)
   and `src/lib/api/schema.d.ts` is updated.
5. **Migration present** — if the data model changed, a migration was added for the right context
   (`ef-migration` skill) and the model snapshot is updated.
6. **Backend builds** — `dotnet build` → 0 errors, and no **new** analyzer/style warnings introduced by
   the change (see `coding-standards`).
7. **Formatting clean** — `dotnet format AEspejo.Clinic.sln --verify-no-changes` → no diff.
8. **Frontend lints** — `cd src/AEspejo.Clinic.Client && npm run lint` → clean.
9. **No stray artifacts** — no throwaway test entities/enums/migrations, no `console.log` debugging left
   in changed `.tsx`/`.ts`.

## Output

Report each item as pass/fail with the specific offending files, then fix the failures and re-run the
relevant checks. Only report "ready" when 1–7 pass.
