namespace ShComp.Updater;

public class UpdaterConfiguration
{
    public bool IsEnabled { get; set; }

    public string? ExeFileName { get; set; }

    public string? SourcePath { get; set; }

    public string? ProductionPath { get; set; }
}
