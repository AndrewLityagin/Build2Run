using Build2RunContract;

namespace Build2Run;

public class Settings(
    string sourceFolder, 
    string targetFolder, 
    int minutesAfterBuild, 
    string fileMask,
    bool killBlockProcess,
    bool replaceBinaryFiles): ISettings
{
    public required string SourceFolder { get; set; } = sourceFolder;
    
    public required string TargetFolder { get; set; } = targetFolder;

    public required int MinutesAfterBuild { get; set; } = minutesAfterBuild;

    public required string FileMask { get; set; } = fileMask;
    
    public bool KillBlockProcess { get; set; } = killBlockProcess;
    
    public bool ReplaceBinaryFiles { get; set; } = replaceBinaryFiles;

    public override string ToString()
    {
        return $"Source folder: {this.SourceFolder};\n" +
               $"Target folder: {this.TargetFolder};\n" +
               $"Minutes after build: {this.MinutesAfterBuild};\n" +
               $"File masks: {this.FileMask};\n" +
               $"Replace binary files : {this.ReplaceBinaryFiles};\n" +
               $"Kill block process: {this.KillBlockProcess};";
    }
}