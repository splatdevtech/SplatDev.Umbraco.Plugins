namespace SplatDev.DigitalBookCurator.Core.Models;

public class CuratorSettings
{
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public bool DeleteEmptyFolders { get; set; }
}
