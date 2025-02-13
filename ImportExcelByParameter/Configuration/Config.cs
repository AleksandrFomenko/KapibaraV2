using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using KapibaraConfig = KapibaraCore.Configuration;

namespace ImportExcelByParameter.Configuration;

internal class Config
{
    private static readonly string DllPath = Assembly.GetExecutingAssembly().Location;
    private static readonly string DllDirectory = Path.GetDirectoryName(DllPath);
    private string _configFilePath = Path.Combine(DllDirectory, "ImportExcelConfig", "config.json");
    
    public string PathStr { get; set; }
    public string ListStr { get; set; }
    public int Number { get; set; }
    public string Category { get; set; }
    public string Parameter { get; set; }

    internal Config()
    {
        var directoryName = "ImportExcelConfig";
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
        KapibaraConfig.Configuration.CreateEmptyJsonFile(dirCfg, configName);
        Category = string.Empty;
        ListStr = string.Empty;
        Number = 1;
        Parameter = string.Empty;
        PathStr = string.Empty;
    
        SaveConfig();
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
