using System.Text.Json;

namespace Build2Run;

public class ConfigurationReaderJson
{
    public Settings[]? Read(string filePath)
    { 
        var json = File.ReadAllText(filePath);
            var configs = JsonSerializer.Deserialize<Settings[]>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
            return configs;
    }
}