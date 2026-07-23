---
name: i18n-audit
description: Audit AEspejo.Clinic for the two recurring defect classes — hardcoded user-visible strings in the frontend, missing/mismatched i18n keys between en.json and es.json, and non-English source comments. Use before finishing a change, during review, or when the user asks to check translations/i18n coverage.
---

# i18n & language audit

Checks the project's two hard rules: **no hardcoded UI text** and **all comments in English**, plus
**locale key parity**. Run the checks, report findings, then fix.

Frontend root: `src/AEspejo.Clinic.Client`. Locales: `src/lib/i18n/locales/en.json`, `es.json`.

## 1. Hardcoded user-visible text in `.tsx`

Flag literal JSX text and human-readable string props not going through `t()`:
```
# Literal text between tags (e.g. >Save Changes<)
rg -n '>[A-Za-z][A-Za-z ,.!?]{2,}<' src/AEspejo.Clinic.Client/src --glob '*.tsx'
# Human-readable string props
rg -n '(placeholder|title|label|aria-label)="[^"]*[A-Za-z]{3}' src/AEspejo.Clinic.Client/src --glob '*.tsx'
```
Review hits: anything a user reads must be `t('group.key')`. Ignore non-text props (`className`, `type`,
`role`, `id`, icon names, test ids).

### Recurring blind spots

These slip past review repeatedly — check them explicitly:

- **`aria-label` / `title` on icon buttons.** Easy to miss because there's no visible text. Past
  offenders: `components/ui/dialog.tsx` (was `aria-label="Cerrar"` — a Spanish literal too),
  `components/ui/Toast.tsx`, `routes/AppLayout.tsx`. All must be `t('common.*')`.
- **Error / not-found / fallback pages.** `pages/NotFoundPage.tsx` and `components/ErrorBoundary.tsx`
  are outside the CRUD registry, so their copy is hand-written and tends to stay hardcoded.
  Keys live under `notFound.*` and `error.*`.
- **Class components can't call `useTranslation()`.** `ErrorBoundary` is a class — extract the visible
  UI into a small functional sub-component (e.g. `ErrorFallback`) that uses the hook, and render it
  from the class.
- **Enums shown as plain text.** A registry column with an enum value but no `kind:'enum'`/`enumName`
  renders the raw English value (e.g. `AuditAction` → "Created"). Every displayed enum needs an i18n
  group in both locales, an entry in `ENUM_VALUES` (`lib/api/types.ts`), and `kind:'enum'` on the column.

## 2. Locale key parity (en.json vs es.json)

Every key in one file must exist in the other. Quick check:
```
node -e "const en=require('./src/AEspejo.Clinic.Client/src/lib/i18n/locales/en.json'),es=require('./src/AEspejo.Clinic.Client/src/lib/i18n/locales/es.json');const f=(o,p='')=>Object.entries(o).flatMap(([k,v])=>v&&typeof v==='object'?f(v,p+k+'.'):[p+k]);const E=new Set(f(en)),S=new Set(f(es));const only=(a,b)=>[...a].filter(x=>!b.has(x));console.log('only in en:',only(E,S));console.log('only in es:',only(S,E));"
```
Both arrays must be empty.

## 3. Non-English comments in source

```
rg -n '(//|///|\*)' --glob '*.cs' --glob '*.ts' --glob '*.tsx' -g '!**/Migrations/**' -g '!**/schema.d.ts' \
  | rg -iw '(el|la|los|las|del|para|con|que|una|por|si|es|son|debe|usar|campo|tabla|cadena|solo|desde|cuando|sin|cada|este|esta|pero|como|vez|añade|crea|devuelve|permite)'
```
Any hit is a Spanish comment to translate to English.

## Fix

- Hardcoded text → wrap in `t('group.key')` and add the key to **both** locale files (see the frontend
  `CLAUDE.md` for which group).
- Parity gaps → add the missing key to whichever file lacks it.
- Spanish comments → translate to English in place.

Re-run the checks until all three are clean.
