dotnet new install Umbraco.Templates::17.4.2
# Create solution/project
dotnet new sln --name "Umbraco.Baseline"
dotnet new umbraco --force -n "Umbraco.Baseline.Web"
dotnet sln add "Umbraco.Baseline.Web"
dotnet run --project "Umbraco.Baseline.Web"
#Running