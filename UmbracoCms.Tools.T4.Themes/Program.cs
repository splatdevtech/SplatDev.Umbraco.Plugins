using System.CommandLine;
using UmbracoCms.Tools.T4.Themes;

var rootCommand = new RootCommand("Umbraco theme scaffolding code generator");

var generateCommand = new Command("generate", "Scaffold a new Umbraco theme");
var nameOpt = new Option<string>("--name", "Theme name (PascalCase)") { IsRequired = true };
var outputOpt = new Option<string>("--output", () => "./src", "Target output directory");
generateCommand.AddOption(nameOpt);
generateCommand.AddOption(outputOpt);

generateCommand.SetHandler((name, output) =>
{
    var scaffolder = new ThemeScaffolder(name, output);
    scaffolder.Scaffold();
    Console.WriteLine($"Theme '{name}' scaffolded to: {output}");
    return Task.CompletedTask;
}, nameOpt, outputOpt);

rootCommand.AddCommand(generateCommand);

return await rootCommand.InvokeAsync(args);
