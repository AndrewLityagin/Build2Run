namespace Build2Run;

public class Settings(
    string configurationName,
    string sourceFolder, 
    string targetFolder, 
    int minutesAfterBuild, 
    string fileMask,
    bool killBlockProcess,
    bool replaceBinaryFiles)
{
    public required string ConfigurationName { get; set; } = configurationName;
    
    public required string SourceFolder { get; set; } = sourceFolder;
    
    public required string TargetFolder { get; set; } = targetFolder;

    public required int MinutesAfterBuild { get; set; } = minutesAfterBuild;

    public required string FileMask { get; set; } = fileMask;
    
    public bool KillBlockProcess { get; set; } = killBlockProcess;
    
    public bool ReplaceBinaryFiles { get; set; } = replaceBinaryFiles;

    public override string ToString()
    {
        return  $"Configuration name: {this.ConfigurationName};\n" +
                $"\t- Source folder: {this.SourceFolder};\n" +
                $"\t- Target folder: {this.TargetFolder};\n" +
                $"\t- Minutes after build: {this.MinutesAfterBuild};\n" +
                $"\t- File masks: {this.FileMask};\n" +
                $"\t- Replace binary files : {this.ReplaceBinaryFiles};\n" +
                $"\t- Kill block process: {this.KillBlockProcess};";
    }
}