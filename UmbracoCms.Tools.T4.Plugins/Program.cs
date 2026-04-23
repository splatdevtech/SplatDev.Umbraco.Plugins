using System.CommandLine;
using UmbracoCms.Tools.T4.Plugins;

var rootCommand = new RootCommand("Umbraco plugin scaffolding code generator");

var generateCommand = new Command("generate", "Scaffold a new Umbraco plugin");
var nameOpt = new Option<string>("--name", "Plugin name (PascalCase)") { IsRequired = true };
var outputOpt = new Option<string>("--output", () => "./src", "Target output directory");
var namespaceOpt = new Option<string>("--namespace", "Root namespace (defaults to plugin name)");
generateCommand.AddOption(nameOpt);
generateCommand.AddOption(outputOpt);
generateCommand.AddOption(namespaceOpt);

generateCommand.SetHandler((name, output, ns) =>
{
    ns = string.IsNullOrWhiteSpace(ns) ? name : ns;
    var scaffolder = new PluginScaffolder(name, ns, output);
    scaffolder.Scaffold();
    Console.WriteLine($"Plugin '{name}' scaffolded to: {output}");
    return Task.CompletedTask;
}, nameOpt, outputOpt, namespaceOpt);

rootCommand.AddCommand(generateCommand);

return await rootCommand.InvokeAsync(args);
