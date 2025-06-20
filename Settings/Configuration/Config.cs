using System.IO;
using System.Reflection;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using Settings.Models;
using KapibaraConfig = KapibaraCore.Configuration;

namespace Settings.Configuration;

public class Config
{
    private static readonly string DllPath = Assembly.GetExecutingAssembly().Location;
    private static readonly string DllDirectory = Path.GetDirectoryName(DllPath);
    private string _configFilePath = Path.Combine(DllDirectory, "SettingsConfig", "config.json");
    
    public Setting Setting { get; set; }

    public Config()
    {
        var directoryName = "SettingsConfig";
        var configName = "config.json";
        
        // path to dll
        var dllPath = Assembly.GetExecutingAssembly().Location;
        KapibaraConfig.Configuration.CreateDir(dllPath, directoryName);
        
        // path to dll`s directory
        var dllDir = Path.GetDirectoryName(dllPath);
        
        // path to cfg`s directory
        var pathCfg = Path.Combine(dllDir, directoryName, configName);
        
        // path to cfg`s
        
        var dirCfg = Path.Combine(dllDir, directoryName);
        
        if (!File.Exists(pathCfg))
        {
            KapibaraConfig.Configuration.CreateEmptyJsonFile(dirCfg, configName);
            Setting = new Setting("Светлая", Theme.Light);
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
            TaskDialog.Show("Error", $"Ошибка с сохранние конфигурации: {ex.Message}");
        }
    }
}
