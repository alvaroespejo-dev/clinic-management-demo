---
name: coding-standards
description: Apply and verify the AEspejo.Clinic coding standard — .NET 10 analyzers + code style enforced by .editorconfig and Directory.Build.props, plus frontend linting. Use when writing/reviewing C# code, when the user asks about conventions/best practices, or before finishing a change.
---

# Coding standard (.NET 10)

The standard is **tooling-enforced**, not just documented. Two files at the repo root drive it:

- `.editorconfig` — C#/.NET code style, naming conventions, and analyzer severities.
- `Directory.Build.props` — enables `EnableNETAnalyzers`, `AnalysisLevel=latest-recommended`, and
  `EnforceCodeStyleInBuild=true` for every `.csproj` (conditioned so the JS `esproj` is untouched).

Style/analyzer violations surface as **build warnings** (not errors) — the build stays green, but keep the
warning count at zero for changed code.

## Verify & apply

```
dotnet build AEspejo.Clinic.sln          # analyzers + style run here (warnings)
dotnet format AEspejo.Clinic.sln         # auto-apply formatting + fixable style
dotnet format AEspejo.Clinic.sln --verify-no-changes   # CI/pre-commit check (no diff = pass)
```
Frontend: `cd src/AEspejo.Clinic.Client && npm run lint` (oxlint).

## Conventions (enforced by `.editorconfig`)

- **File-scoped namespaces**; usings outside the namespace; System directives first.
- **Nullable enabled**; prefer pattern matching, null-propagation, `readonly` fields.
- **Naming**: interfaces `I*`; types/methods PascalCase; locals/params camelCase; private instance fields
  `_camelCase`; const & `static readonly` fields PascalCase.
- Language keywords over BCL names (`string` not `String`).
- Primary constructors, target-typed `new`, collection expressions `[..]` are all accepted style.

## Project idioms (beyond formatting)

- Services return `Result<T>` / `Result`; controllers map with `ApiResults` (`ToActionResult`).
- Constructor injection (primary constructors); async methods take a `CancellationToken`.
- Reuse the generic CRUD stack before writing bespoke code (see `add-entity` / `custom-endpoint`).
- Comments in English; no hardcoded user-visible strings (see `i18n-audit`).

## Tuned severities (intentional)

`.editorconfig` silences a few rules that fight this codebase's deliberate patterns:
`CA1000` (static factories on `Result<T>`), `CA1716` (`IAppDbContext.Set<T>()` mirrors EF), and — in
`*Tests.cs` only — `CA1707`/`IDE1006` (underscore test names). Don't re-enable these without cause.
`TenantProvisioningService.ProvisionAsync` keeps a `CA1822` warning on purpose: it's an injectable
service, not a static helper.
