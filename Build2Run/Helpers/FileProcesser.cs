using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Mono.Cecil;
 
namespace Build2Run;

public static class FileProcesser
{
    public static (BinaryFileInfo[] binaryFiels, BinaryFileInfo[] instanceFiles) GetAllFiles(string binaryFolder, string instanceFolder, int minutes, string fileMask)
    {
        var instanceFiles = GetFiles(instanceFolder, fileMask);
        var binaryFiles = GetFiles(binaryFolder, fileMask, minutes);
        return (binaryFiles, instanceFiles);
    }
    
    private static BinaryFileInfo[] GetFiles(string folder, string fileMask, int minutes = 0)
    {
        var directoryInfo = new DirectoryInfo(folder);

        var currentTime = (DateTime.Now).AddMinutes(minutes * (-1));
        
        var filePattern = new Regex(fileMask, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
        var files = minutes > 0 ? directoryInfo.GetFiles("*", SearchOption.AllDirectories)
                .Where(f => filePattern.IsMatch(f.Name))
                .Where(f => f.LastWriteTime >= currentTime)
                .ToArray()
            : directoryInfo.GetFiles("*", SearchOption.AllDirectories)
                .Where(f => filePattern.IsMatch(f.Name))
                .ToArray();

        var binaryFiles = new List<BinaryFileInfo>();
        foreach(var file in files)
        {
            try
            {
                using(AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(file.FullName))
                {
                    var customAttribute = assembly.CustomAttributes.FirstOrDefault(c => c.AttributeType.Name == typeof(TargetFrameworkAttribute).Name);
                    binaryFiles.Add(new BinaryFileInfo()
                    {
                        Info = file,
                        AssemblyName = assembly.Name.Name,
                        AssemblyVersion = assembly.Name.Version,
                        TargetFramework = customAttribute != null ? (string)customAttribute.ConstructorArguments[0].Value : string.Empty,
                    });
                }
            }
            catch
            {
                binaryFiles.Add(new BinaryFileInfo()
                {
                    Info = file,
                    AssemblyName = string.Empty,
                    AssemblyVersion = null,
                    TargetFramework = string.Empty,
                });
            }
        }
        return binaryFiles.ToArray();
    }
    
    public static void ReplaceFile(BinaryFileInfo source, BinaryFileInfo target, ref int replacedNumber, ref int isNotReplacedNumber, List<(BinaryFileInfo, BinaryFileInfo)>? isNotReplacedFiles)
    {
        try
        {
            File.Copy(source.Info.FullName, target.Info.FullName, true);
            replacedNumber++;
            
            var sourcePdbFileName = source.Info.FullName.Replace(".dll", ".pdb");
            var targetPdbFileName = target.Info.FullName.Replace(".dll", ".pdb");

            if(File.Exists(sourcePdbFileName))
                File.Copy(sourcePdbFileName, targetPdbFileName, true);

            if (isNotReplacedFiles == null)
                isNotReplacedNumber--;
        }
        catch (Exception ex)
        {
            if (isNotReplacedFiles != null)
            {
                isNotReplacedFiles.Add((source, target));
                isNotReplacedNumber++;
            }
        }
    }

    public static (int replaced,int notReplaced) ReplaceAllFilesInFolders(string source, string target)
    {
        var replaced = 0;
        var notReplaced = 0;
        var files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
        foreach(var file in files)
        {
            try
            {
                var newFileName = file.Replace(source, target);
                File.Delete(newFileName);
                File.Copy(file,newFileName);
                replaced++;
            }
            catch
            {
                notReplaced++;
            }
        }
        return  (replaced,notReplaced);
    }
}