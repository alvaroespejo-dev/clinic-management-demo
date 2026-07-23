# Feature requirements

Product/feature specs for AEspejo.Clinic. Each feature gets one Markdown file here so the intended
behavior is written down before (and while) it is built.

## How to use

1. Copy `TEMPLATE.md` to `docs/requirements/<feature-slug>.md` (e.g. `appointment-reminders.md`).
2. Fill in every section. The template's checklist and business rules are what turn into
   validators, tests, and acceptance checks.
3. When asking Claude Code to implement the feature, point it at the spec file — it should read the
   spec first and follow the project conventions in the root `CLAUDE.md` (generic CRUD pattern,
   comments in English, all UI text via i18n in `en.json` + `es.json`).
4. Keep the spec updated if the scope changes; it's the source of truth for what "done" means.

## Conventions to remember in every spec

- Prefer the generic CRUD pattern (`CrudControllerBase` / `CrudServiceBase` / `registry.ts`) unless
  the feature genuinely needs custom logic — say which in the API section.
- State whether the change touches the **master DB** or the **tenant DB**, and whether a migration is needed.
- List new i18n keys explicitly; they must be added to both locale files.
