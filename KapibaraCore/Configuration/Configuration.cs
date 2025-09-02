using System.IO;
using Autodesk.Revit.UI;
using Newtonsoft.Json;

namespace KapibaraCore.Configuration;

public static class Configuration
{
    public static void CreateDir(string dllPath, string directoryName)
    {
        var dllDirectory = Path.GetDirectoryName(dllPath);
        var configDirectoryPath = Path.Combine(dllDirectory, directoryName);
        if (!Directory.Exists(configDirectoryPath))
        {
            var x = Directory.CreateDirectory(configDirectoryPath);
        }
    }
    public static void CreateEmptyJsonFile(string directoryPath, string fileName)
    {
        var filePath = Path.Combine(directoryPath, fileName);
        
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "{}");
        }
    }


    public static T LoadConfig<T>(string filePath) where T : class
    {
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл конфигурации не найден", filePath);
            }
            
            var json = File.ReadAllText(filePath);
            var config = JsonConvert.DeserializeObject<T>(json);

            return config;
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"File Not Found :{ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"An error occurred while deserializing the JSON: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            return null;
        }
    }
}