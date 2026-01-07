// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Build2Run;
using Build2RunContract;

try
{   
    Console.Clear();
    Console.WriteLine("#====================================================================================#");
    Console.WriteLine("#                                 Build2Run ver 1.0                                  #");
    Console.WriteLine("#====================================================================================#");
    
    Console.WriteLine("1. Read configuration file...");
    var configurationReader = new ConfigurationReaderJson();
    var rawConfig = configurationReader.Read(args[0]);
    
    if(rawConfig == null)
        throw new Exception("No configuration found.");
    
    var configuration = (Settings)rawConfig;
    Console.WriteLine("Configuration loaded:\n" + configuration.ToString());
    
    if (configuration.ReplaceBinaryFiles)
        ReplaceBinaryFiles(configuration);
    else
        ReplaceFolders(configuration);

}
catch (Exception e)
{
    Console.WriteLine("Exception: " + e.Message);
}

return 0;


void ReplaceBinaryFiles(Settings config)
{
    Console.WriteLine("\n2. Looking for binary files in source and target folders...");
    var(binaryFiles, instanceFiles) =  FileProcesser.GetAllFiles(config.SourceFolder, config.TargetFolder, config.MinutesAfterBuild, config.FileMask);
    Console.WriteLine($"Found in source : {binaryFiles.Length}\nFound in target : {instanceFiles.Length}");
    var replacedNumber = 0;
    var isNotReplacedNumber = 0;
    
    var isNotReplacedFiles = new List<(BinaryFileInfo, BinaryFileInfo)>();
    Console.WriteLine("\n3. Comparing and replacing files are started...");
    foreach(var sourceFile in binaryFiles)
    {
        List<BinaryFileInfo> filesToReplace;

        if(sourceFile.IsUnreadableAssembly)
            filesToReplace = instanceFiles.Where(ftr => ftr.IsUnreadableAssembly)
                .Where(ftr => ftr.Info.Name == sourceFile.Info.Name).ToList();
        else
            filesToReplace = instanceFiles.Where(ftr => ftr.AssemblyName == sourceFile.AssemblyName
                                                        && ftr.AssemblyVersion == sourceFile.AssemblyVersion
                                                        && ftr.TargetFramework == sourceFile.TargetFramework).ToList();
        
        Console.WriteLine($"\nReplacing the file {sourceFile.Info.Name} to:");
        
        foreach (var targetFile in filesToReplace)
        {
            Console.WriteLine($"-> {targetFile.Info.DirectoryName}\\");
            FileProcesser.ReplaceFile(sourceFile, targetFile, ref replacedNumber, ref isNotReplacedNumber, isNotReplacedFiles);
        }
    }

    if (config.KillBlockProcess)
    {
        if (isNotReplacedFiles.Any())
        {
            Console.WriteLine($"\nFiles are not replaced:{isNotReplacedNumber}\nTry again...\n");
            var processes = new Dictionary<int, string>();
            
            foreach (var files in isNotReplacedFiles)
            {
                var tempDict = FileLockerHelper.GetLockerProcess(files.Item2.Info.FullName);
                foreach (var process in tempDict)
                    processes.TryAdd(process.Key,process.Value);
            }

            Console.WriteLine($"Close processes:");
            foreach (var process in processes)
            {
                Console.WriteLine($"- {process.Value} ({process.Key})");
                FileLockerHelper.CloseProcess(process.Key);
            }

            Thread.Sleep(1000);
            Console.WriteLine($"\nReplacing: ");
            foreach (var files in isNotReplacedFiles)
            {
                Console.WriteLine($"{files.Item1.Info.Name} -> {files.Item2.Info.FullName}");
                FileProcesser.ReplaceFile(files.Item1, files.Item2, ref replacedNumber, ref isNotReplacedNumber, null);
            }
        }
    }
    
    Console.WriteLine($"\nFiles are replaced: {replacedNumber}");
    Console.WriteLine($"Files are not replaced: {isNotReplacedNumber}");
}

void ReplaceFolders(Settings config)
{
    Console.WriteLine($"\n 2. Replacing files from  {config.SourceFolder} -> {config.TargetFolder}");
    var(replaced,notReplaced) = FileProcesser.ReplaceAllFilesInFolders(config.SourceFolder, config.TargetFolder);
    Console.WriteLine($"Files are replaced: {replaced}\nFiles are not replaced: {notReplaced}");
}