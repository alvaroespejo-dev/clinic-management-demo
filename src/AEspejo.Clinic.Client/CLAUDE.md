# AEspejo.Clinic.Client (frontend)

React + Vite + TypeScript SPA. Data layer is TanStack Query over a typed API client.
See the repo-root `CLAUDE.md` for backend/architecture context.

## Hard rules

1. **All comments in English.**
2. **No hardcoded user-visible text.** Every label, button, toast, dialog, placeholder and validation
   message must be an i18n key rendered with `t('...')`. This was a real bug source — treat any literal
   string in JSX that a user can read as a defect.

## i18n workflow

- Locales: `src/lib/i18n/locales/en.json` and `src/lib/i18n/locales/es.json`. **Always edit both.**
- Add keys under the right group: `common.*` (shared), `nav.*` (menu/routes), `fields.*` (entity fields,
  keyed by DTO property name), `settings.*`, `crud.*`, or the enum groups (`UserRole`, `Gender`, ...).
- Render: `const { t } = useTranslation()` then `t('group.key')`. Interpolate with `t('k', { name })`
  and `"...{{name}}..."` in the JSON.
- Field labels resolve automatically as `t(\`fields.${key}\`)`; enum cells as `t(\`${enumName}.${value}\`)`.

## Generic CRUD (mirror of the backend)

The whole admin UI is generated from one registry — you rarely write a new page.

- `src/features/registry.ts` — central array of `EntityConfig`. Menu and routes are generated from it.
- `src/features/config.ts` — `EntityConfig` shape and reusable reference-row labels
  (`labelName`, `labelFullName`, `labelCode`).
- `src/components/shared/CrudPage.tsx` — renders list + create/edit dialog for any config.
- `src/lib/api/crud.ts` — `createCrud<TResponse,TCreate,TUpdate>(resource)` → TanStack Query hooks.

To add an entity to the UI, append an `EntityConfig`:
- `key` (route segment) · `resource` (REST resource) · `navKey` (`nav.*`) · `group` (`nav.group*`) · optional `adminOnly`.
- `columns`: list columns; each `{ key, kind? }` where `kind` ∈ `bool | enum | money | date | datetime`
  (+ `enumName` for enums).
- `fields`: form fields; `kind` ∈ `text | textarea | checkbox | password | enum | reference | datetime`.
  Use `only: 'create' | 'edit'` to scope a field, `required: true`, and for FKs
  `reference: { resource, label }`.
- `defaults`: initial create-form values.
- Add matching `nav.*` and `fields.*` keys to **both** locale files.

## API types

After changing backend DTOs, run `npm run gen:api` (API must be up on :5287) to regenerate
`src/lib/api/schema.d.ts`. Hand-written mirror types live in `src/lib/api/types.ts`.

## UI building blocks

- Primitives: `src/components/ui/primitives.tsx` (`Button`, `Input`, `Card`, `Select`, `Badge`, ...).
- Feedback: toasts via `useToast()`, confirm dialogs via `useConfirmDialog()`, `EmptyState`, `SkeletonLoader`.
- Don't introduce a new UI library or a new modal/toast system — reuse these.
