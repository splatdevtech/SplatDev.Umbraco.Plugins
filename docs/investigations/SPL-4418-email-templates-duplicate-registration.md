# SPL-4418 — Email Templates duplicate extension registration

## Finding

The duplicate registration is caused by two delivery paths for the same manifest:

1. `SplatDev.Umbraco.Plugins.EmailTemplates.csproj` embeds `App_Plugins/**` and the generated embedded manifest proves that `SplatDev.EmailTemplates/umbraco-package.json` is inside the DLL.
2. `buildTransitive/SplatDev.Umbraco.Plugins.EmailTemplates.targets` copies the same source `App_Plugins/**` into the consuming application's `App_Plugins/SplatDev.EmailTemplates` directory before build.

The assembly is therefore loaded with an embedded manifest while the host also has the physical manifest. The four aliases in the source manifest are declared only once; the duplication is delivery-time, not duplicate declarations.

The existing `EmbeddedPackageManifestReader` correctly avoids adding its embedded manifest when the physical copy exists, which protects upgrades that retain a stale copied folder. It does not make the copy safe for a fresh install, because Umbraco independently discovers the physical manifest and the embedded/static-assets path can still enumerate it twice.

The generated embedded resource manifest (`obj/Release/net10.0/Microsoft.Extensions.FileProviders.Embedded.Manifest.xml`) contains:

- `App_Plugins/SplatDev.EmailTemplates/umbraco-package.json`
- `App_Plugins/SplatDev.EmailTemplates/dist/email-templates-dashboard.js`
- its source map

This confirms the RCL/NuGet assembly ships the complete plugin UI without needing a content-copy target.

## Change

Removed the transitive content-copy target. The package now has one authoritative asset delivery path: embedded App_Plugins assets, served by `EmbeddedAppPluginsComposer`; on Umbraco 17 the embedded manifest reader registers extensions when no legacy physical copy exists.

Existing installations that already have a physical folder remain safe because `HasPhysicalCopy` deliberately lets the physical manifest win. Operators should remove the old `App_Plugins/SplatDev.EmailTemplates` folder once upgrading from a release that copied it, or leave it in place until the next clean deployment.

## Verification

- Build both target frameworks for the plugin.
- Inspect the generated embedded manifest and package contents; confirm the DLL contains the manifest and UI assets and no build-transitive copy target remains.
- The staging console logs from `qa-runs/SPL-4409-2026-10-05/` are the baseline evidence: all routes reported the four duplicate aliases before this fix.

A staging redeploy and clean host verification are still required to confirm that the four console errors disappear in the deployed sandbox.
