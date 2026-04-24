using System.CommandLine;
using SplatDev.Umbraco.Tools.Packager;

var rootCommand = new RootCommand("Umbraco NuGet packager CLI");

var packCommand = new Command("pack", "Build package manifest, bundle assets, and run dotnet pack");
var packNameOpt = new Option<string>("--name", "Package name") { IsRequired = true };
var packVersionOpt = new Option<string>("--version", () => "1.0.0", "Package version");
var packOutputOpt = new Option<string>("--output", () => "./nupkg", "Output directory");
packCommand.AddOption(packNameOpt);
packCommand.AddOption(packVersionOpt);
packCommand.AddOption(packOutputOpt);
packCommand.SetHandler(async (name, version, output) =>
{
    var builder = new PackageBuilder(name, version, output);
    await builder.BuildAsync();
}, packNameOpt, packVersionOpt, packOutputOpt);

var manifestCommand = new Command("manifest", "Generate App_Plugins package manifest only");
var manifestNameOpt = new Option<string>("--name", "Package name") { IsRequired = true };
var manifestVersionOpt = new Option<string>("--version", () => "1.0.0", "Package version");
var manifestOutputOpt = new Option<string>("--output", () => ".", "Output directory");
manifestCommand.AddOption(manifestNameOpt);
manifestCommand.AddOption(manifestVersionOpt);
manifestCommand.AddOption(manifestOutputOpt);
manifestCommand.SetHandler((name, version, output) =>
{
    var gen = new ManifestGenerator(name, version);
    gen.Generate(output);
    return Task.CompletedTask;
}, manifestNameOpt, manifestVersionOpt, manifestOutputOpt);

var cleanCommand = new Command("clean", "Remove generated App_Plugins output and nupkg artifacts");
var cleanOutputOpt = new Option<string>("--output", () => "./nupkg", "Output directory to clean");
cleanCommand.AddOption(cleanOutputOpt);
cleanCommand.SetHandler((output) =>
{
    if (Directory.Exists(output))
        Directory.Delete(output, recursive: true);
    Console.WriteLine($"Cleaned: {output}");
    return Task.CompletedTask;
}, cleanOutputOpt);

rootCommand.AddCommand(packCommand);
rootCommand.AddCommand(manifestCommand);
rootCommand.AddCommand(cleanCommand);

return await rootCommand.InvokeAsync(args);
