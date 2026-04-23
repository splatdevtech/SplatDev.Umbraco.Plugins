using System.Diagnostics;

namespace UmbracoCms.Tools.Packager;

public class PackageBuilder
{
    private readonly string _name;
    private readonly string _version;
    private readonly string _outputDir;

    public PackageBuilder(string name, string version, string outputDir)
    {
        _name = name;
        _version = version;
        _outputDir = outputDir;
    }

    public async Task BuildAsync()
    {
        var workDir = Directory.GetCurrentDirectory();

        var generator = new ManifestGenerator(_name, _version);
        generator.Generate(workDir);

        var bundler = new AssetBundler(workDir, workDir);
        bundler.Bundle();

        Directory.CreateDirectory(_outputDir);

        var psi = new ProcessStartInfo("dotnet", $"pack --output \"{_outputDir}\" /p:Version={_version}")
        {
            WorkingDirectory = workDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        using var process = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start dotnet pack.");

        var stdout = await process.StandardOutput.ReadToEndAsync();
        var stderr = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (!string.IsNullOrWhiteSpace(stdout))
            Console.WriteLine(stdout);
        if (!string.IsNullOrWhiteSpace(stderr))
            Console.Error.WriteLine(stderr);

        if (process.ExitCode != 0)
            throw new InvalidOperationException($"dotnet pack failed with exit code {process.ExitCode}.");

        Console.WriteLine($"Package created in: {_outputDir}");
    }
}
