# SplatDev Umbraco Plugins — Master Implementation Plan

**Date:** 2026-05-27
**Deadline:** July 31, 2026
**Status:** Draft — awaiting board approval

---

## Overview

This is the master plan for implementing all SplatDev Umbraco plugins across Umbraco 13 (AngularJS) and Umbraco 17 (Lit 3 / Bellissima). The full catalog of plugins is in `docs/specs/2026-05-27-all-plugins-catalog.md`.

**Tech standards:**
- Umbraco 13: .NET 8, AngularJS backoffice
- Umbraco 17: .NET 10, Lit 3 / Bellissima backoffice
- Single multi-target NuGet package per plugin (`net8.0;net10.0`) where possible
- Separate GitHub repo per plugin (under https://github.com/orgs/SplatDev-Ltda)
- NuGet published to NuGet.org + Umbraco Marketplace
- All specs and plans live under `docs/specs/` and `docs/plans/` within each plugin folder

---

## Phase 0 — Foundations (Weeks 1–2)

### Goal: Complete the two flagship plugins (Backups + Workflow) and the Yaml plugin that powers theme installation.

| Plugin | Umbraco 13 | Umbraco 17 | Individual Spec | Individual Plan |
|---|---|---|---|---|
| **Backups** | Add AngularJS UI | Build Lit 3 UI | `BackupManager/docs/specs/2026-05-27-backup-v13-design.md` | `BackupManager/docs/plans/2026-05-27-backup-v13-plan.md` |
| **Backups** | ✅ Backend complete | ✅ Backend complete | `BackupManager/docs/specs/2026-05-27-backup-v17-design.md` | `BackupManager/docs/plans/2026-05-27-backup-v17-plan.md` |
| **Workflow** | Implement from spec | Implement from spec | `Workflow/docs/specs/2026-05-26-workflow-plugin-design.md` | `Workflow/docs/plans/2026-05-26-workflow-plugin-v1-plan.md` |
| **Workflow v17** | — | Port backoffice to Lit 3 | `Workflow/docs/specs/2026-05-27-workflow-v17-design.md` | `Workflow/docs/plans/2026-05-27-workflow-v17-plan.md` |
| **Yaml** | Verify/complete | Port backoffice to Lit 3 | Existing in `Yaml/docs/` | Existing in `Yaml/docs/` |
| **FormBuilder** | Create spec + implement | Create spec + implement | TBD (new spec needed) | TBD (new plan needed) |

**Definition of Done for Phase 0:**
- All Phase 0 plugins pass `dotnet build` clean on both `net8.0` and `net10.0`
- All unit + E2E tests pass
- NuGet packages buildable and publishable
- Both Umbraco 13 and Umbraco 17 dashboards functional

---

## Phase 1 — Security & Member Infrastructure (Weeks 3–4)

### Goal: Port the security, auth, and member management plugins to Umbraco 17.

| Plugin | Task |
|---|---|
| `SplatDev.Umbraco.Plugins.Security` | Audit current state, create spec, implement v17 backoffice if needed |
| `SplatDev.Umbraco.Plugins.2fa` | Audit, create spec for Umbraco 17, implement Lit 3 UI |
| `SplatDev.Umbraco.Plugins.CustomLogin` | Spec + implement custom login page for Umbraco 17 |
| `SplatDev.Umbraco.Plugins.PasswordSettings` | Spec + implement password policy settings for Umbraco 17 |
| `SplatDev.Umbraco.Authorization.Ldap` | Verify net10.0 compatibility; update to Umbraco 17 |
| `SplatDev.Umbraco.Plugins.OAuth` | Spec + implement OAuth integration for Umbraco 17 |
| `SplatDev.Umbraco.Plugins.MemberLogin` | Spec + implement member login components for v17 |
| `SplatDev.Umbraco.Plugins.MemberRegistration` | Spec + implement member registration for v17 |
| `SplatDev.Umbraco.Plugins.MemberGroups` | Verify/port to v17 |
| `SplatDev.Umbraco.Plugins.MemberTypes` | Verify/port to v17 |
| `SplatDev.Umbraco.Plugins.AdminBar` | Spec + implement fixed admin bar for v17 |
| `SplatDev.Umbraco.Plugins.CacheManager` | Port backoffice dashboard to Lit 3 for v17 |
| `SplatDev.Umbraco.Plugins.RedirectManager` | Port backoffice to Lit 3 for v17 |
| `SplatDev.Umbraco.Plugins.CodeFirst` | Compare with Yaml, merge unique features, retire or mark archived |

**Definition of Done for Phase 1:**
- All plugins compile on net8.0 + net10.0
- Umbraco 17 dashboards/backoffice functional
- Tests passing

---

## Phase 2 — Content, Communication & Commerce (Weeks 5–7)

### Goal: Port medium-priority content and commerce plugins.

**Content & CMS plugins:**
- `SEO`, `Settings`, `DictionaryManager`, `EmailNotifications`, `Mailer`, `Smtp`
- `Blog`, `PhotoGallery`, `ExceptionManager`, `PropertiesReport`
- `CharLimit`, `CopyValue`, `DefaultValue`, `Dropzone`, `ExamineExtensions`
- `EmailTemplates`, `ShortUrls`, `HiddenContent`, `Analytics`
- `Newsletter`, `Newsletters`, `ToastNotifications`

**Commerce plugins:**
- `ShopCart`, `Payments.BancoInter`, `Payments.MercadoPago`, `Payments.PagSeguro`

**Forms:**
- `FormsClone`, `Surveys`

**Integration:**
- `D4Sign`, `ENotAssina`, `SocialMedia.Channels`

**For each plugin in Phase 2:**
1. Audit existing v13 code
2. Create spec document: `<Plugin>/docs/specs/<date>-<plugin>-design.md`
3. Create plan document: `<Plugin>/docs/plans/<date>-<plugin>-plan.md`
4. Implement missing pieces (backend or UI)
5. Port backoffice to Lit 3 for Umbraco 17
6. Add tests

---

## Phase 3 — Themes (Weeks 7–9)

### Goal: Complete all themes for both Umbraco 13 and Umbraco 17.

**Prerequisite:** Yaml plugin (Phase 0) and FormBuilder (Phase 0) must be complete first, as themes use Yaml for installation.

| Theme | Target |
|---|---|
| `Themes.Base` | Foundation for all other themes |
| `Themes.Corporate` | Corporate/business website |
| `Themes.Blog` | Blog site |
| `Themes.Commerce` | E-commerce site |
| `Themes.Landing` | Landing page |
| `Themes.Conference` | Conference/event |
| `Themes.Forum` | Community forum |
| `Themes.Hotel` | Hotel/hospitality |
| `Themes.Portfolio` | Portfolio/creative |

**Theme structure for Umbraco 17:**
Each theme is a NuGet package that:
1. Registers via `umbraco-package.json` as a Bellissima extension
2. Installs content types via the Yaml plugin
3. Provides a starter content set

---

## Phase 4 — Niche Plugins (Weeks 9–11)

**Low-priority, on-demand:**
- `StarRatings`, `Countries`, `OnOff`, `Exif`, `LazyLoad`, `MostViewed`
- `NewsTicker`, `Slider`, `VideoPreview`, `SvgViewer`, `PdfCurator`, `VisitorCounter`
- `Tweets`, `WhatsApp`, `SocialMedia.Share`, `LiveVideo`
- `QuickPoll`, `Rsvp`, `Faqs`, `Forums`
- `IspServices`, `WordsApi`, `JsonRpc`, `RdpManager`

---

## Phase 5 — NuGet Publishing & Marketplace (Week 11–12)

### Goal: Publish all completed plugins.

For each plugin:
1. Run full test suite
2. Build NuGet package: `dotnet pack -c Release`
3. Push to NuGet.org: `dotnet nuget push`
4. Create GitHub repository under `https://github.com/orgs/SplatDev-Ltda`
5. Submit to Umbraco Marketplace: https://marketplace.umbraco.com/
6. Generate product icon and README banner
7. Write/update `README.md` with install instructions + screenshots

---

## Per-Plugin Spec/Plan Requirements

Before any plugin implementation begins (Phase 1+), the following must exist:

### Spec document (`docs/specs/<date>-<pluginname>-design.md`)
- Goals & non-goals
- Current implementation state
- Architecture (solution layout + project boundaries)
- API surface (if applicable)
- Backoffice UI spec (if applicable: v13 AngularJS + v17 Lit 3)
- Configuration schema
- Testing strategy
- Deliverables

### Plan document (`docs/plans/<date>-<pluginname>-plan.md`)
- Phased tasks with checkbox steps
- File structure
- Code snippets for each task
- Acceptance criteria

---

## Timeline Summary

| Phase | Scope | Duration | Target Date |
|---|---|---|---|
| 0 | Backups + Workflow + Yaml + FormBuilder | 2 weeks | 2026-06-10 |
| 1 | Security + Members + Admin tools | 2 weeks | 2026-06-24 |
| 2 | Content + Commerce + Communication | 3 weeks | 2026-07-15 |
| 3 | Themes (all 9) | 2 weeks | 2026-07-29 |
| 4 | Niche plugins | 2 weeks | 2026-07-29 |
| 5 | NuGet + Marketplace publishing | 2 weeks | 2026-07-31 |

**Phases 3, 4, and 5 run partially in parallel.** Target date: **July 31, 2026** ✅

---

## Team Assignments

| Phase | Primary | Support |
|---|---|---|
| 0 — Backups v13 frontend | Backend Developer | QA |
| 0 — Backups v17 Lit 3 | Frontend Specialist | Backend Developer |
| 0 — Workflow v13 (build) | Backend Developer | Frontend Specialist |
| 0 — Workflow v17 (Lit 3 port) | Frontend Specialist | Backend Developer |
| 0 — Yaml + FormBuilder | Backend Developer | QA |
| 1 — Security & Members | Backend Developer | Frontend Specialist |
| 2 — Content & Commerce | Backend Developer | Frontend Specialist |
| 3 — Themes | Frontend Specialist | Backend Developer |
| 4 — Niche plugins | Backend Developer | — |
| 5 — Publishing | DevOps Specialist | CTO |

---

## Open Questions for Board

1. **Priority of FormBuilder**: Should FormBuilder be a full clone of Umbraco Forms (100% feature parity) or a targeted subset covering 80% of use cases? Feature parity will take significantly longer.

2. **Theme customization**: Should themes be installable via the Umbraco backoffice UI (wizard), or only via `dotnet add package`? The Yaml-based installation supports both but the backoffice wizard requires additional development.

3. **CodeFirst vs Yaml**: CodeFirst is described as "old implementation" and Yaml is the new one. Should CodeFirst be:
   a. Retired (mark as archived)
   b. Merged into Yaml (any unique features absorbed)
   c. Maintained in parallel

4. **Phase 4 priority ordering**: Which niche plugins should be tackled first if time allows? Is there any client/project dependency driving the order?

5. **Localization scope**: Instructions say to include English and Spanish. Should Portuguese (Brazil) also be required for all plugins given the SplatDev audience?
