# SPL-4420 — Dashboard rendering investigation

**Date:** 2026-10-06  
**Status:** Blocked pending SPL-4419 (clean registration-fix verification)

## Finding

The duplicate registration is confirmed as a dual delivery path for the Email Templates backoffice assets, not as duplicate aliases in the JSON source:

1. `SplatDev.Umbraco.Plugins.EmailTemplates.csproj` embeds every file below `App_Plugins/**` as an `EmbeddedResource`.
2. The assembly contains the embedded `App_Plugins`, `umbraco-package.json`, and `email-templates-dashboard` resource strings (verified against the Release `net10.0` DLL).
3. `EmbeddedAppPluginsComposer` registers an `IPackageManifestReader` that reads the embedded `umbraco-package.json` and composes an embedded file provider.
4. The package's build-transitive target also copies `App_Plugins/**` into the consuming application's `App_Plugins` directory.
5. The staging evidence records a physical `/app/wwwroot/App_Plugins/SplatDev.EmailTemplates` folder while the plugin DLL is also deployed in `/app`.

Therefore the same manifest is offered by the physical Umbraco `App_Plugins` scan and by the assembly's embedded manifest reader. The four reported aliases are declared exactly once in source, but are registered twice at runtime. The current composer has a physical-copy guard; the retained staging evidence predates or does not include a clean verification of that fix.

## Route evidence currently available

The retained run visited 21 exposed routes. It proves route exposure/navigation, not successful plugin rendering. Every retained route console log contains the same registration errors, so rendering classification is not trustworthy until SPL-4419 removes those errors:

| Route | Current evidence | Classification now | Exact blocker |
|---|---|---|---|
| `redirect-management` | navigated; screenshot + console log | inconclusive | duplicate extension registration |
| `json-rpc` | navigated; screenshot + console log | inconclusive | duplicate extension registration |
| `email-notifications` | navigated; screenshot + console log | inconclusive | duplicate extension registration |
| `santander-banking` | navigated; screenshot + console log | inconclusive | duplicate extension registration |
| `enotassina` | navigated; screenshot + console log | inconclusive | duplicate extension registration |
| `d4sign` | navigated; screenshot + console log | inconclusive | duplicate extension registration |
| `banco-inter` | navigated; screenshot + console log | inconclusive | duplicate extension registration |
| `toast-notifications` | navigated; screenshot + console log | inconclusive | duplicate extension registration |
| `live-video` | navigated; screenshot + console log | inconclusive | duplicate extension registration |
| `video-preview` | navigated; screenshot + console log | inconclusive | duplicate extension registration |
| `rdp-manager` | navigated; screenshot + console log | inconclusive | duplicate extension registration |
| `gdrp` | navigated; screenshot + console log | inconclusive | duplicate extension registration |
| `social-media-share` | navigated; screenshot + console log | inconclusive | duplicate extension registration |
| `social-media-login` | navigated; screenshot + console log | inconclusive | duplicate extension registration |
| `properties-report` | navigated; screenshot + console log | inconclusive | duplicate extension registration |
| `site-settings` | navigated; screenshot + console log | inconclusive | duplicate extension registration |
| `shop-cart` | navigated; screenshot + console log | inconclusive | duplicate extension registration |
| `passwordsettings` | navigated; redirected/rendered generic `dashboardTabsContentIntro` shell | shell observed, root cause not separable | duplicate extension registration |
| `restricted` | navigated; screenshot + console log | inconclusive | duplicate extension registration |
| `hiddencontent` | navigated; screenshot + console log | inconclusive | duplicate extension registration |
| `splatdev-analytics` | navigated; title was `Password Settings | Umbraco` | inconclusive | duplicate extension registration |

## What remains

Re-run the table after SPL-4419 on a clean console/network capture. For each route record whether its own element mounted, whether its dashboard bundle returned 200, and the exact plugin-specific text. Do not treat the existing generic shell result as proof that the plugin UI is absent: the global duplicate-registration defect occurs on every route.
