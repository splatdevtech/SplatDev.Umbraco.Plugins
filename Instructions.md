# Projects Realm

## IMPORTANT
     - Some projects are already partially implemented inside '/mnt/e/source/repos/SplatDev Corporate Umbraco v11 2026'
     - Abstract plugins from Findlay Project (Like Azure Entra and Active Directory, etc) - in '/mnt/e/Source/Repos/Findlay Auto'
     - Abstract plugins from QuoteTab Project - in '/mnt/e/Source/Repos/Pinoter/QuoteTab Umbraco v13'

## Branch Strategy
* Major branches: `u13` (Umbraco 13, net8.0, AngularJS backoffice) and `u17` (Umbraco 17, net10.0, Lit 3 backoffice)
* Feature branches: `feature/SPL-XXXX-description` — branched from `u13` or `u17` depending on target
* Bugfix branches: `fix/SPL-XXXX-description` — same convention
* Never commit directly to `u13`, `u17`, or `master`
* Archived branches live under `archive/` — revive by branching off the appropriate major branch

* Guidelines:
    + User Umbraco elements (Tags)
    + Lit 3 for backoffice plugins (angularjs for umbraco 13)
    + Create nuget packages for all projects
    + Create separate github repositories for each project (under https://github.com/orgs/SplatDev-Ltda)
    + Publish packages to Umbraco Marketplace
    + Use Context7 to get latest documentation
    + Generate icons and product images
    + Generate documentation and README.md

+ Backup plugin:
 - Implement cloud storage with Google Cloud, Box, Dropbox, OneDrive, S3, Azure Storage, etc.
 - For version 13, use angularjs, for version 17 use Lit 3
 - There will be a version for Umbraco 13 and a version for Umbraco 17 (keep both in the same repository, different branches)

+ Umbraco FormBuilder is a clone of Umbraco Forms, but with no licensing requirements
+ Umbraco Themes use Umbraco-Yaml project in order to install and run theme installations.
+ Umbraco Code-First is an old implementation of what now is Umbraco-Yaml. Compare both old and new and add any functionalities to yaml project if applicable.
+ Umbraco Admin bar is a fixed header above body content (right below body tag) with admin shortcuts for Logged in users.

+ Some projects (helpers, etc) are not umbraco plugins but nuget packages to help speed up development, and not as actual plugins to use with Umbraco.