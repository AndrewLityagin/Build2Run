namespace Build2RunContract;

public interface ISettings
{
    string SourceFolder { get; set; }
    string TargetFolder { get; set; }
    int MinutesAfterBuild { get; set; }
    string FileMask { get; set; }
    bool KillBlockProcess { get; set; }
    bool ReplaceBinaryFiles { get; set; }
}