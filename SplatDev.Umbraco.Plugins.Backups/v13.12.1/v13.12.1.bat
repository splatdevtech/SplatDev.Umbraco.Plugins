dotnet new install Umbraco.Templates::13.12.1
# Create solution/project
dotnet new sln --name "SplatDev.uPlugins.Backups"
dotnet new umbraco --force -n "SplatDev.uPlugins.Backups.Web_Sqlite"
dotnet sln add "SplatDev.uPlugins.Backups.Web_Sqlite"
dotnet new umbraco --force -n "SplatDev.uPlugins.Backups.Web_Mssql"
dotnet sln add "SplatDev.uPlugins.Backups.Web_Mssql"
dotnet new umbracopackage --force -n "SplatDev.uPlugins.Backups"
dotnet sln add "SplatDev.uPlugins.Backups"
#Running