namespace Build2Run;

public sealed class BinaryFileInfo
{
    public required FileInfo Info { get; set; }

    public string? TargetFramework { get; set; }

    public string? AssemblyName { get; set; }

    public Version? AssemblyVersion { get; set; }

    public bool IsUnreadableAssembly => string.IsNullOrEmpty(AssemblyName) || AssemblyVersion == null || string.IsNullOrEmpty(TargetFramework);

    public override string ToString() => $"File: {Info?.FullName} | AssemblyVersion: {AssemblyVersion} | TargetFramework: {TargetFramework}";
}