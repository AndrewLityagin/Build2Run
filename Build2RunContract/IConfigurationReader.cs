namespace Build2RunContract;

public interface IConfigurationReader
{
    ISettings? Read(string filePath);
}