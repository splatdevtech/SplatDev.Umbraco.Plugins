Simple Backups

for Umbraco v13.2.0

Install via nuget

		Install-Package SplatDev.Umbraco.Plugins.Backups

A simple backup tool for Umbraco. Allows backing up files and database directly from the backoffice, when you don't have access to the server.

- Creates a Dashboard to perform backups from the backoffice
- It also works when using SQLCE

Specs
- Type: Dashboard
- Value Type: NONE
- Dependencies:
  - Umbraco.Cms.Web.Website
  - Umbraco.Cms.Web.BackOffice
  - System.Data.SqlClient