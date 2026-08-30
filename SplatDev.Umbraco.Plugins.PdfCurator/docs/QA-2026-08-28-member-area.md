# PdfCurator Member Area QA Report

**Issue:** SPL-3286  
**Initial date:** 2026-08-28  
**Rerun:** 2026-08-29  
**Environment:** `https://staging-umbraco.splatdev.tech` (Umbraco 17 staging)  
**Result:** **Blocked — authenticated acceptance cannot be completed against the deployed build**

## Executive result

The member-area source has client unit-test and bundle evidence, but the staging prerequisite is not met. The deployed MemberLogin endpoint still throws `System.NotSupportedException: IMemberSignInManager is not available in Umbraco 17`, and the PdfCurator member API is unavailable (`404`). Therefore no authenticated acceptance criterion, member-state persistence claim, WCAG browser audit, i18n browser verification, or screenshot pass may be claimed.

Required unblock: deploy MemberLogin 2.1.4 from commit `8c1faebf` (`review/SPL-3778`) and deploy/seed the PdfCurator member-area fixture, then rerun this report's blocked checks.

## Evidence collected

| Check | Result | Evidence |
|---|---|---|
| MemberLogin invalid-shaped request | **FAIL / blocker** | On 2026-08-29, `POST /umbraco/api/memberlogin/login` returned HTTP 500, `System.NotSupportedException: IMemberSignInManager is not available in Umbraco 17`, from `MemberLoginService.LoginAsync`. |
| MemberLogin empty payload validation | **PASS (route reachability only)** | On 2026-08-29, empty payload returned HTTP 400 validation JSON (`Password` and `Username` required). This is not authentication evidence. |
| PdfCurator anonymous member API | **BLOCKED** | On 2026-08-29, `GET /umbraco/pdfcurator/api/v1/member/books` returned HTTP 404. |
| MemberLogin static manifest | **NOT deployment evidence** | On 2026-08-29, `GET /App_Plugins/MemberLogin/umbraco-package.json` returned HTTP 200 and reports static version `2.0.0`; static asset serving does not prove the corrected server package is deployed. |
| Member client unit tests | **PASS** | On 2026-08-29, `npm test -- --run`: 4 files, 24 tests passed. jsdom emitted expected canvas `getContext` not-implemented warnings while reader tests ran. |
| Member bundle budget | **PASS (source build)** | On 2026-08-29, `npm run build:member`: `member.js` 35,653 bytes raw / 6.81 KB gzip, below the 80 KB gzip budget. Reader remains split into lazy chunks (`reader` and `reader-vendor`). |
| .NET PdfCurator tests (net10) | **PASS** | On 2026-08-29, target-specific Release test command completed: 24 passed, 0 failed, 0 skipped. |
| .NET PdfCurator tests (net8) | **FAIL / compatibility defect** | The cross-target command still fails compiling `MemberSimilarControllerTests.cs` and `SimilarBooksScoringTests.cs` because PdfCurator member types/controllers are excluded from the net8 surface. |

## Acceptance matrix

| Criterion | Status | Reason |
|---|---|---|
| AC1 — login, library, search/filter, anonymous 401/redirect | **BLOCKED** | Login fails HTTP 500 before authentication; member API returns 404. |
| AC2 — reader, 400+ page pagination, persisted progress, Continue reading | **BLOCKED** | No authenticated member session, seeded 400+ page fixture, or reachable member API. |
| AC3 — favorites persistence and Reading Now | **BLOCKED** | No authenticated session or reachable API. |
| AC4 — sane similar-books rail | **BLOCKED** | No reachable member detail/similar endpoint or seeded catalog. |
| AC5 — MemberGroupScopes 404/visibility behavior | **BLOCKED** | No authenticated scoped/unscoped fixture or reachable endpoint. |
| AC6 — bundle, axe, keyboard, i18n | **PARTIAL** | Bundle budget passes and source inspection found keyboard handlers, dialog semantics, focus-trap implementation, and en/es dictionaries. Browser axe/WCAG and rendered-language checks require the deployed authenticated page. |
| Regression — Phase A backoffice | **BLOCKED** | No authenticated staging backoffice smoke run was possible; no dedicated PdfCurator Playwright fixture is present in this workspace. |

## Static accessibility and i18n audit

The source contains positive indicators:

- Library cards and similar cards are keyboard-operable with `Enter` and `Space`, and use `tabindex="0"`.
- Reader uses `role="dialog"`, a focusable container, Escape-to-close, Tab focus trapping, and keyboard page navigation.
- Reader controls and inputs have accessible labels; progress uses `role="progressbar"`; status feedback uses `role="status"` and `aria-live="polite"`.
- Library covers use title-derived alt text; book covers use title-derived alt text.
- English and Spanish dictionaries include library, book, favorite, similar-rail, and reader strings.

The following remain unverified or should be reviewed during the rerun:

- `axe-core` zero-violation result on the rendered page cannot be established without a working authenticated page.
- Keyboard/focus behavior must be exercised in a real browser, not inferred solely from source.
- Several navigation and pagination labels (`Filter by category`, `Sort books`, `Previous page`, `Next page`, `First page`, `Last page`, `Go to page`, `PDF Reader`) are hard-coded English strings rather than dictionary entries. This is an i18n audit finding to fix or explicitly accept before release.
- Similar/favorite thumbnails use empty alt text while their containing controls have labels. This may be valid for decorative duplication, but should be confirmed in the axe/browser pass against the approved interpretation of the alt-text requirement.

## Commands

```text
# Frontend
npm test -- --run
npm run build:member

# Net10 target (passes)
dotnet test SplatDev.Umbraco.Plugins.PdfCurator.Tests/SplatDev.Umbraco.Plugins.PdfCurator.Tests.csproj -c Release --no-restore -f net10.0 --filter "Category!=Integration&Category!=InDevelopment"

# Both-target command (net8 fails as documented above)
dotnet test SplatDev.Umbraco.Plugins.PdfCurator.Tests/SplatDev.Umbraco.Plugins.PdfCurator.Tests.csproj -c Release --no-restore --filter "Category!=Integration&Category!=InDevelopment"
```

## Heartbeat verification — 2026-08-30 03:03 UTC

A fresh anonymous staging probe and local verification were run during this heartbeat. Results remain unchanged:

- Invalid-shaped MemberLogin request: **HTTP 500**, `System.NotSupportedException: IMemberSignInManager is not available in Umbraco 17 (net10.0)`.
- Anonymous PdfCurator member books: **HTTP 404**.
- MemberLogin static manifest: **HTTP 200**, version **2.0.0** (asset serving only; not proof of server-package deployment).
- Frontend tests: **24/24 pass** across 4 files. jsdom emitted the existing expected canvas `getContext` warnings.
- Member production build: `member.js` **35.65 kB raw / 6.81 kB gzip**; reader remains lazy-loaded.
- Target-specific PdfCurator tests: **24/24 pass** on net10.0. The run emitted existing NuGet vulnerability warnings; no test failures.

Authenticated AC1–AC5, browser axe/WCAG, rendered i18n, screenshots, and Phase A regression remain environment-blocked. The deployment prerequisite is still MemberLogin **2.1.4** from commit `8c1faebf` (`review/SPL-3778`) plus deployed/seeded PdfCurator member fixtures.

## Rerun checklist after deployment

1. Verify MemberLogin returns a normal authentication result using approved QA credentials; do not use invalid fixtures as acceptance evidence.
2. Seed/confirm the 400+ page PDF, Technology books, member groups, scoped and unscoped members.
3. Run AC1–AC5 in a clean browser context and capture screenshots for library, reader/progress, favorites, similar rail, and scope behavior.
4. Run axe-core on library/book/reader states and manually verify keyboard focus trap and Escape behavior.
5. Verify en/es rendered strings, including all labels and error states.
6. Run Phase A backoffice smoke checks and record API response evidence.
7. Resolve the net8 test compilation failure or document a supported-target exclusion before release sign-off.
8. Remove all seeded test data after the run.

## Heartbeat verification — 2026-08-29 12:21 UTC

A fresh anonymous probe was run before closing this QA pass. Results are unchanged:

- Invalid-shaped MemberLogin request: **HTTP 500**, `System.NotSupportedException: IMemberSignInManager is not available in Umbraco 17 (net10.0)` from `MemberLoginService.LoginAsync`.
- Empty MemberLogin payload: **HTTP 400** with required `Username`/`Password` validation errors (route reachability only).
- Anonymous PdfCurator member books: **HTTP 404**.
- MemberLogin static manifest: **HTTP 200**, version **2.0.0** (asset serving only; not proof of server-package deployment).

The local evidence remains reproducible: frontend tests **24/24 pass**, `build:member` emits `member.js` **35.65 kB raw / 6.81 kB gzip** (reader remains lazy-loaded), and the target-specific net10 test suite passes **24/24**. Authenticated AC1–AC5, rendered browser WCAG/axe, rendered i18n, screenshots, and Phase A regression remain environment-blocked. No authenticated acceptance claim is made.

**Required unblock:** deploy MemberLogin **2.1.4** from commit `8c1faebf` (`review/SPL-3778`) and deploy/seed PdfCurator member fixtures, then rerun the authenticated checklist and browser evidence.

## Heartbeat verification — 2026-08-30 03:11 UTC

A fresh anonymous staging probe and local verification were run during this heartbeat. Results remain unchanged:

- Invalid-shaped MemberLogin request (`Username`/`Password` supplied): **HTTP 500**, `System.NotSupportedException: IMemberSignInManager is not available in Umbraco 17 (net10.0)`.
- Empty MemberLogin payload: **HTTP 400** with required-field validation (route reachability only).
- Anonymous PdfCurator member books: **HTTP 404**.
- MemberLogin static manifest: **HTTP 200**, version **2.0.0** (asset serving only; not proof of server-package deployment).
- Frontend tests: **24/24 pass** across 4 files.
- Member production build: `member.js` **35.65 kB raw / 6.81 kB gzip**; reader remains lazy-loaded.
- Target-specific PdfCurator tests: **24/24 pass** on net10.0. The run emitted existing NuGet vulnerability warnings; no test failures.

Authenticated AC1–AC5, browser axe/WCAG, rendered i18n, screenshots, and Phase A regression remain environment-blocked. Per the established default, this issue remains blocked pending deployment of MemberLogin **2.1.4** (`8c1faebf`) and seeded PdfCurator member fixtures.

## Heartbeat verification — 2026-08-30 11:54 UTC

A fresh anonymous staging probe and local verification were completed during this heartbeat; the blocker remains active:

- MemberLogin empty payload: **HTTP 400** with required `Username`/`Password` validation errors (route reachability only).
- MemberLogin invalid credentials with a valid-shaped request: **HTTP 500**, `System.NotSupportedException: IMemberSignInManager is not available in Umbraco 17 (net10.0)`.
- Anonymous PdfCurator member books: **HTTP 404**.
- MemberLogin static manifest: **HTTP 200**, version **2.0.0** (asset serving only; not server-package deployment evidence).
- Frontend Vitest: **24/24 passed** across 4 files; only the existing jsdom canvas `getContext` warnings were emitted.
- Member production build: `member.js` **35.65 kB raw / 6.81 kB gzip**, under the 80 KB gzip budget; reader remains lazy-loaded.
- Target-specific PdfCurator tests: **24/24 passed** on `net10.0`; existing NuGet vulnerability warnings remain.

Authenticated AC1–AC5, browser axe/WCAG, rendered i18n, screenshots, and Phase A regression remain unclaimable. The blocker clears when MemberLogin **2.1.4** from `8c1faebf` (`review/SPL-3778`) and deployed/seeded PdfCurator member fixtures are available; the authenticated rerun checklist above then applies.


## Heartbeat verification — 2026-08-30 03:18 UTC

A fresh QA heartbeat reran the reproducible local checks and anonymous staging probes. Results remain unchanged:

- Frontend client tests: **24/24 pass** across 4 files. Vitest emitted the existing jsdom canvas `getContext` warnings only.
- Member production bundle: **35.65 kB raw / 6.81 kB gzip** for `member.js`, below the 80 KB gzip budget; reader remains in lazy-loaded chunks.
- Target-specific PdfCurator .NET tests: **24/24 pass** on `net10.0`; existing NuGet vulnerability warnings were emitted.
- Cross-target PdfCurator test command: **fails at net8.0 compilation** in the existing member similar-book tests because the member surface is intentionally excluded from the plugin's net8 target.
- Staging MemberLogin invalid-shaped request: **HTTP 500**, `IMemberSignInManager is not available in Umbraco 17`.
- Staging MemberLogin empty payload: **HTTP 400** required-field validation (route reachability only).
- Staging anonymous member books: **HTTP 404**.
- Staging MemberLogin static manifest: **HTTP 200**, version **2.0.0** (asset serving only, not server-package evidence).

Authenticated AC1–AC5, browser axe/WCAG, rendered i18n, screenshots, and Phase A regression remain environment-blocked. The required unblock is deployment of MemberLogin **2.1.4** from commit `8c1faebf` (`review/SPL-3778`) plus deployed and seeded PdfCurator member fixtures.

## Heartbeat verification — 2026-08-30 11:45 UTC

A fresh reproducible verification was completed during this heartbeat:

- Frontend client tests: **24/24 pass** across 4 files. Vitest emitted only the existing jsdom canvas `getContext` warnings.
- Member production build: **35.65 kB raw / 6.81 kB gzip** for `member.js`, below the 80 KB gzip budget; reader remains lazy-loaded.
- Target-specific PdfCurator .NET tests: **24/24 pass** on `net10.0`; existing NuGet vulnerability warnings remain.
- Staging MemberLogin invalid-shaped request with string credentials: **HTTP 500**, `IMemberSignInManager is not available in Umbraco 17`.
- Staging MemberLogin empty payload: **HTTP 400** required-field validation (route reachability only).
- Staging anonymous member books: **HTTP 404**.
- Staging MemberLogin static manifest: **HTTP 200**, version **2.0.0** (asset serving only, not server-package evidence).

Authenticated AC1–AC5, browser axe/WCAG, rendered i18n, screenshots, and Phase A regression remain environment-blocked. The required unblock is deployment of MemberLogin **2.1.4** from commit `8c1faebf` (`review/SPL-3778`) plus deployed and seeded PdfCurator member fixtures. Per the established default, this issue remains blocked pending that deployment.

## Heartbeat verification — 2026-08-30 21:12 UTC

A fresh QA heartbeat reran the reproducible local checks and anonymous staging probes. Results remain unchanged:

- Frontend Vitest: **24/24 passed** across 4 files; only the existing jsdom canvas `getContext` warnings were emitted.
- Member production build: `member.js` **35.65 kB raw / 6.81 kB gzip**, below the 80 KB gzip budget; reader remains lazy-loaded into separate chunks.
- Target-specific PdfCurator .NET tests: **24/24 passed** on `net10.0`; existing NuGet vulnerability warnings remain, with no test failures.
- Staging MemberLogin valid-shaped invalid credentials: **HTTP 500**, `System.NotSupportedException: IMemberSignInManager is not available in Umbraco 17`.
- Staging MemberLogin empty payload: **HTTP 400** required-field validation (route reachability only).
- Staging anonymous PdfCurator member books: **HTTP 404**.
- Staging MemberLogin static manifest: **HTTP 200**, version **2.0.0** (asset serving only; not server-package deployment evidence).

Authenticated AC1–AC5, browser axe/WCAG, rendered i18n, screenshots, and Phase A regression remain environment-blocked. The blocker clears when MemberLogin **2.1.4** from commit `8c1faebf` (`review/SPL-3778`) and deployed/seeded PdfCurator member fixtures are available; rerun the authenticated checklist above afterward.

## Heartbeat verification — 2026-08-30 21:12 UTC (scheduler rerun)

A fresh anonymous staging probe was repeated during this heartbeat; deployment evidence remains unchanged:

- MemberLogin valid-shaped invalid credentials: **HTTP 500**, `System.NotSupportedException: IMemberSignInManager is not available in Umbraco 17 (net10.0)`.
- MemberLogin static manifest: **HTTP 200**, version **2.0.0** (asset serving only; not server-package deployment evidence).
- Anonymous PdfCurator member books: **HTTP 404**.
- Local frontend tests: **24/24 passed**; only existing jsdom canvas warnings were emitted.
- Member production bundle: **35.65 kB raw / 6.81 kB gzip**, under budget; reader remains lazy-loaded.
- Target-specific PdfCurator tests: **24/24 passed** on `net10.0`; existing NuGet vulnerability warnings remain.

Authenticated AC1–AC5, browser axe/WCAG, rendered i18n, screenshots, and Phase A regression remain blocked. Clear by deploying MemberLogin **2.1.4** from commit `8c1faebf` (`review/SPL-3778`) and deployed/seeded PdfCurator member fixtures, then rerun the authenticated checklist.
