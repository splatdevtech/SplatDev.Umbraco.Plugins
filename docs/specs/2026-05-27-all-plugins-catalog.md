# SplatDev Umbraco Plugins — Full Catalog

**Date:** 2026-05-27
**Status:** Draft for board approval

This document catalogs all plugins and libraries in this repository, their current implementation status, their target Umbraco versions, and their priority tier for the Umbraco 17 port.

---

## Legend

| Symbol | Meaning |
|---|---|
| ✅ | Complete — implemented, tested, published |
| 🔶 | Partial — code exists, some pieces missing (spec, tests, frontend) |
| ❌ | Not started |
| N/A | Not applicable (not a UI plugin or no Umbraco 17 backoffice needed) |

---

## A — Backup & Storage

| Plugin | v13 Status | v17 Status | Spec | Plan | Priority |
|---|---|---|---|---|---|
| `SplatDev.Umbraco.Plugins.Backups` | 🔶 (missing AngularJS UI) | 🔶 (missing Lit 3 UI) | ✅ See `BackupManager/docs/specs/` | ✅ See `BackupManager/docs/plans/` | P0 |

---

## B — Workflow

| Plugin | v13 Status | v17 Status | Spec | Plan | Priority |
|---|---|---|---|---|---|
| `SplatDev.Umbraco.Workflow` | ❌ Not built yet | ❌ Not built yet | ✅ `Workflow/docs/specs/` (both v13 + v17) | ✅ `Workflow/docs/plans/` (both v13 + v17) | P0 |

---

## C — Core Infrastructure (shared libraries, not UI plugins)

| Library | Description | Umbraco Version | Status | Notes |
|---|---|---|---|---|
| `SplatDev.Umbraco.Common` | Shared extensions, helpers | 13 + 17 | 🔶 | Verify net10.0 target |
| `SplatDev.Umbraco.EntityFramework` | EF Core integration helpers | 13 + 17 | 🔶 | Verify net10.0 target |
| `SplatDev.Umbraco.NPoco` | NPoco extensions for Umbraco | 13 only | ✅ | Not needed for v17 |
| `SplatDev.Umbraco.Membership` | Custom membership helpers | 13 + 17 | 🔶 | |
| `SplatDev.Umbraco.Examine` | Examine/Lucene helpers | 13 + 17 | 🔶 | |
| `SplatDev.Umbraco.Pagination` | Paging helpers | 13 + 17 | 🔶 | |
| `SplatDev.Umbraco.Markup` | HTML/Razor markup utilities | 13 + 17 | 🔶 | |
| `SplatDev.Umbraco.QueryStringFilters` | Query string filter helpers | 13 + 17 | 🔶 | |
| `SplatDev.Umbraco.DataTypes.USStates` | US States data type | 13 + 17 | 🔶 | Minimal UI |

---

## D — Security & Authentication

| Plugin | v13 Status | v17 Status | Priority |
|---|---|---|---|
| `SplatDev.Umbraco.Plugins.2fa` | 🔶 | ❌ | P1 |
| `SplatDev.Umbraco.2FA` | 🔶 | ❌ | P1 |
| `SplatDev.Umbraco.Plugins.Security` | 🔶 | ❌ | P1 |
| `SplatDev.Umbraco.Plugins.CustomLogin` | 🔶 | ❌ | P1 |
| `SplatDev.Umbraco.Plugins.PasswordSettings` | 🔶 | ❌ | P1 |
| `SplatDev.Umbraco.Authorization.Ldap` | ✅ (Findlay) | ❌ | P1 |
| `SplatDev.Umbraco.Plugins.OAuth` | 🔶 | ❌ | P1 |
| `SplatDev.Umbraco.Plugins.Restricted` | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.Gdrp` | 🔶 | ❌ | P2 |

---

## E — Member Management

| Plugin | v13 Status | v17 Status | Priority |
|---|---|---|---|
| `SplatDev.Umbraco.Plugins.MemberLogin` | 🔶 | ❌ | P1 |
| `SplatDev.Umbraco.Plugins.MemberRegistration` | 🔶 | ❌ | P1 |
| `SplatDev.Umbraco.Plugins.MemberGroups` | 🔶 | ❌ | P1 |
| `SplatDev.Umbraco.Plugins.MemberTypes` | 🔶 | ❌ | P1 |
| `SplatDev.Umbraco.Plugins.MemberNotifications` | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.SocialMedia.Login` | 🔶 | ❌ | P2 |

---

## F — Communication & Notifications

| Plugin | v13 Status | v17 Status | Priority |
|---|---|---|---|
| `SplatDev.Umbraco.Plugins.EmailNotifications` | ✅ | ❌ | P1 |
| `SplatDev.Umbraco.Plugins.Mailer` | 🔶 | ❌ | P1 |
| `SplatDev.Umbraco.Plugins.Smtp` | 🔶 | ❌ | P1 |
| `SplatDev.Umbraco.Plugins.Newsletter` | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.Newsletters` | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.ToastNotifications` | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.WhatsApp` | 🔶 | ❌ | P3 |

---

## G — CMS Utilities & Admin Tools

| Plugin | Description | v13 | v17 | Priority |
|---|---|---|---|---|
| `SplatDev.Umbraco.Plugins.AdminBar` | Fixed admin bar above body for logged-in users | 🔶 | ❌ | P1 |
| `SplatDev.Umbraco.Plugins.CacheManager` | Cache management dashboard | ✅ | ❌ | P1 |
| `SplatDev.Umbraco.Plugins.RedirectManager` | Redirect management | ✅ | ❌ | P1 |
| `SplatDev.Umbraco.Plugins.DictionaryManager` | Dictionary/translation manager | 🔶 | ❌ | P1 |
| `SplatDev.Umbraco.Plugins.Settings` | Site settings management | 🔶 | ❌ | P1 |
| `SplatDev.Umbraco.Plugins.SEO` | SEO meta tag management | 🔶 | ❌ | P1 |
| `SplatDev.Umbraco.Plugins.ExceptionManager` | Exception logging dashboard | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.PropertiesReport` | Document type properties report | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.ShortUrls` | Short URL management | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.HiddenContent` | Hidden content management | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.Analytics` | Site analytics dashboard | 🔶 | ❌ | P2 |

---

## H — Forms & Surveys

| Plugin | Description | v13 | v17 | Priority |
|---|---|---|---|---|
| `SplatDev.Umbraco.Plugins.FormBuilder` | Clone of Umbraco Forms (no license) | 🔶 | ❌ | P0 |
| `SplatDev.Umbraco.Plugins.FormsClone` | Legacy forms clone | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.Surveys` | Survey management | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.QuickPoll` | Quick poll widget | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Plugins.Rsvp` | RSVP management | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Plugins.Faqs` | FAQ management | 🔶 | ❌ | P3 |

---

## I — Content & Media

| Plugin | Description | v13 | v17 | Priority |
|---|---|---|---|---|
| `SplatDev.Umbraco.Plugins.Blog` | Blog management + templates | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.PhotoGallery` | Photo gallery management | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.Slider` | Slider/carousel management | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Plugins.VideoPreview` | Video embed/preview | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Plugins.SvgViewer` | SVG media viewer | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Plugins.Exif` | EXIF data viewer/editor | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Plugins.NewsTicker` | Scrolling news ticker | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Plugins.LazyLoad` | Lazy load helper | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Plugins.LiveVideo` | Live video integration | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Plugins.MostViewed` | Most viewed content tracking | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Plugins.PdfCurator` | PDF management | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Plugins.VisitorCounter` | Page visitor counter | 🔶 | ❌ | P3 |

---

## J — Property Editors & Data Types

| Plugin | Description | v13 | v17 | Priority |
|---|---|---|---|---|
| `SplatDev.Umbraco.Plugins.CharLimit` | Character limit property editor | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.CopyValue` | Copy value between properties | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.DefaultValue` | Default value for properties | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.Dropzone` | Drag-and-drop file upload property editor | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.OnOff` | Toggle/on-off property editor | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Plugins.Countries` | Country picker data type | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.DataTypes.USStates` | US States data type | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Plugins.StarRatings` | Star rating property editor | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Plugins.ExamineExtensions` | Examine/search extensions | 🔶 | ❌ | P2 |

---

## K — Commerce & Payments

| Plugin | Description | v13 | v17 | Priority |
|---|---|---|---|---|
| `SplatDev.Umbraco.Plugins.ShopCart` | Shopping cart | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.Payments.BancoInter` | Banco Inter payment gateway | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.Payments.MercadoPago` | MercadoPago payment gateway | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.Payments.PagSeguro` | PagSeguro payment gateway | 🔶 | ❌ | P2 |

---

## L — Social Media & External Integrations

| Plugin | Description | v13 | v17 | Priority |
|---|---|---|---|---|
| `SplatDev.Umbraco.Plugins.SocialMedia.Channels` | Social media channels management | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.SocialMedia.Share` | Social sharing buttons | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Plugins.Tweets` | Twitter/X feed | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Plugins.D4Sign` | D4Sign digital signature integration | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.ENotAssina` | eNotassina digital signature | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.IspServices` | ISP services integration | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Plugins.WordsApi` | Words API integration | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Plugins.RdpManager` | RDP connection manager | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Plugins.JsonRpc` | JSON-RPC endpoint helper | 🔶 | ❌ | P3 |

---

## M — Developer / Infrastructure

| Plugin | Description | v13 | v17 | Priority |
|---|---|---|---|---|
| `SplatDev.Umbraco.Plugins.CodeFirst` | Code-first content types (compare with Yaml) | 🔶 | ❌ | P1 |
| `SplatDev.Umbraco.Plugins.Yaml` | Yaml-based content type installation | ✅ | 🔶 | P0 |
| `SplatDev.Umbraco.Plugins.EmailTemplates` | Email template manager | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Plugins.Forums` | Community forums | 🔶 | ❌ | P3 |

---

## N — Themes

| Theme | Description | v13 | v17 | Priority |
|---|---|---|---|---|
| `SplatDev.Umbraco.Themes.Base` | Base theme scaffold | 🔶 | ❌ | P1 |
| `SplatDev.Umbraco.Themes.Blog` | Blog theme | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Themes.Commerce` | E-commerce theme | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Themes.Conference` | Conference theme | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Themes.Corporate` | Corporate theme | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Themes.Forum` | Forum theme | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Themes.Hotel` | Hotel theme | 🔶 | ❌ | P3 |
| `SplatDev.Umbraco.Themes.Landing` | Landing page theme | 🔶 | ❌ | P2 |
| `SplatDev.Umbraco.Themes.Portfolio` | Portfolio theme | 🔶 | ❌ | P3 |

---

## Summary Counts

| Priority | Count | Examples |
|---|---|---|
| P0 — Critical path, required first | 4 | Backups, Workflow, FormBuilder, Yaml |
| P1 — High value, early release | ~15 | Security, CacheManager, AdminBar, Members, CodeFirst |
| P2 — Medium value | ~25 | Commerce, Forms, SEO, Analytics, Themes |
| P3 — Nice-to-have / niche | ~30 | Tweets, LiveVideo, StarRatings, etc. |

---

## Notes

- All P0/P1 plugins need both Umbraco 13 and Umbraco 17 versions (dual-target `net8.0;net10.0`).
- P2/P3 plugins: Umbraco 13 versions where already implemented; Umbraco 17 on demand.
- Themes require the `Yaml` plugin (P0) to be done first since `Umbraco-Yaml` is the installation mechanism.
- `FormBuilder` (P0) should be designed to replace Umbraco Forms with no licensing requirements; the spec must include a comparison with Umbraco Forms to ensure feature parity for common scenarios.
- `CodeFirst` vs `Yaml`: A dedicated analysis task should compare these two and merge any CodeFirst-only features into the Yaml plugin before v17 work begins.
