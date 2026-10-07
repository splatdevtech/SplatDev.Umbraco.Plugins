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

Existing installations that already have a physical folder remain safe because `HasPhysicalCopy` deliberately lets the physical manifest win. Operators should remove the old `App_Plugins/SplatDev.EmailTemplates` folder once upgrading from a release that copied it, or leave it in place until the next clean deployment. The reproducible cleanup step for a deployment is `rm -rf wwwroot/App_Plugins/SplatDev.EmailTemplates` before starting the newly packaged application; do not edit the running container by hand.

## Related duplicate-path audit

The same audit was performed for the other two folders called out by the staging ownership evidence:

- **AdPreview**: still embeds `App_Plugins/**` and still has a `buildTransitive` target that copies those files into the consuming site's `App_Plugins` directory. It remains a potential duplicate-path package and should receive the same one-path treatment in a separate change.
- **SocialMediaChannels** (`SplatDev.Umbraco.Plugins.SocialMedia.Channels`): also embeds `App_Plugins/**` and still ships a copy target. It has the same potential delivery collision and is likewise not silently considered fixed by this EmailTemplates change.

This change is intentionally limited to EmailTemplates: removing the EmailTemplates target fixes the four reported aliases without changing unrelated plugin delivery behavior.

## Host verification procedure

After publishing this package, deploy to a clean U17 sandbox (or remove only the stale physical folder using the documented cleanup step), then capture an authenticated fresh-console run. Compare the new logs with `qa-runs/SPL-4409-2026-10-05/console-plugin-*.log`; assert that each new log has zero occurrences of `already registered` and navigate to `/umbraco#/email-templates`. The expected visible section/dashboard text is **Email Templates**, with **Templates** and **Style Settings** menu items. Capture the page screenshot and console log as release evidence.

The retained pre-fix logs are baseline evidence only; this repository cannot claim a post-deploy console result until that sandbox run has been executed.

## Verification

- Build both target frameworks for the plugin.
- Inspect the generated embedded manifest and package contents; confirm the DLL contains the manifest and UI assets and no build-transitive copy target remains.
- The staging console logs from `qa-runs/SPL-4409-2026-10-05/` are the baseline evidence: all routes reported the four duplicate aliases before this fix.

A staging redeploy and clean host verification are still required to confirm that the four console errors disappear in the deployed sandbox.
