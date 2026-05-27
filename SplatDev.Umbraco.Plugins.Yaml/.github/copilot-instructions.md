# Copilot Instructions

## Project Guidelines
- Never include NuGet API keys or any secrets in source files, commit messages, or any tracked content. Always store secrets in GitHub Actions secrets (NUGET_API_KEY) or a secure secrets manager.
- In the Umbraco-Yaml repo, keep references to "For Umbraco 13, use v1.x" and "support/umbraco-13 branch" in source files as they serve as redirect messages for users on Umbraco 13 who need the older plugin version.

## NuGet Packaging (Schema2Yaml)
- Use MSBuild pack settings to configure Schema2Yaml theme-specific packaging; set the theme via pack/MSBuild properties at pack time.
- Change only NuGet metadata when applying a theme; do not modify package contents or runtime behavior as part of packaging.
- Name themed packages using the pattern: SplatDev.Umbraco.Plugins.Schema2Yaml.<ThemeName>
- Keep default behavior unchanged when no theme is specified (leave package name and behavior as the default).
- Support exactly one theme per pack run; produce one theme-targeted package per invocation.