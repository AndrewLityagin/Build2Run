using System.Text.Json;
using Build2RunContract;

namespace Build2Run;

public class ConfigurationReaderJson : IConfigurationReader
{
    public ISettings? Read(string filePath)
    { 
        var json = File.ReadAllText(filePath);
            var config = JsonSerializer.Deserialize<Settings>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
            return config;
    }
}