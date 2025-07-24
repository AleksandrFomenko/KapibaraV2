using System.IO;
using System.Reflection;
using Autodesk.Revit.UI;
using Newtonsoft.Json;

namespace EngineeringSystems.Configuration;

public class Config
{
    private static readonly string DllPath = Assembly.GetExecutingAssembly().Location;
    private static readonly string DllDirectory = Path.GetDirectoryName(DllPath);
    private string _configFilePath = Path.Combine(DllDirectory, "EngineeringSystemsConfig", "config.json");
    

    public string UserParameter { get; set; }

    public Config()
    {
        var directoryName = "EngineeringSystemsConfig";
        var configName = "config.json";
        
        // path to dll
        var dllPath = Assembly.GetExecutingAssembly().Location;
        KapibaraCore.Configuration.Configuration.CreateDir(dllPath, directoryName);
        
        // path to dll`s directory
        var dllDir = Path.GetDirectoryName(dllPath);
        
        // path to cfg`s directory
        var pathCfg = Path.Combine(dllDir, directoryName, configName);
        
        // path to cfg`s
        
        var dirCfg = Path.Combine(dllDir, directoryName);
        
        if (!File.Exists(pathCfg))
        {
            KapibaraCore.Configuration.Configuration.CreateEmptyJsonFile(dirCfg, configName);
            UserParameter = string.Empty;
            SaveConfig();
        }
    }

    public string GetPath()
    {
        return _configFilePath;
    }

    public void SaveConfig()
    {
        try
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(_configFilePath, json);
        }
        catch (Exception ex)
        {
            TaskDialog.Show("Error", $"An error occurred while saving the configuration: {ex.Message}");
        }
    }
}