using System.IO;
using System.Reflection;
using System.Text.Json;
using RiserMate.Abstractions;
using RiserMate.Lookups;
using KapibaraConfig = KapibaraCore.Configuration;

namespace RiserMate.Implementation;

public class ConfigRiserMateService : IConfigRiserMate
{
    private static string DllPath => Assembly.GetExecutingAssembly().Location;
    private static readonly string DllDirectory = Path.GetDirectoryName(DllPath) ?? string.Empty;

    private const string DirectoryName = "RiserMateConfig";
    private const string ConfigName = "config.json";
    
    public string ConfigDirectory => Path.Combine(DllDirectory, DirectoryName);
    public string ConfigFilePath  = Path.Combine(DllDirectory, DirectoryName, ConfigName);

    public RiserMateConfig Cfg { get; set; }

    public ConfigRiserMateService()
    {
        if (!File.Exists(ConfigFilePath))
        {
            Cfg = CreateConfigFile();
            SaveConfig();
        }
        Cfg = KapibaraConfig.Configuration.LoadConfig<RiserMateConfig>(ConfigFilePath) ?? CreateConfigFile();
        SaveConfig();
        Cfg.PropertyChanged += (_, __) => SaveConfig();
    }
    
    
    private RiserMateConfig CreateConfigFile()
    {
        KapibaraConfig.Configuration.CreateDir(DllPath, DirectoryName);
        KapibaraConfig.Configuration.CreateEmptyJsonFile(ConfigDirectory, ConfigName);
        var cfg = new RiserMateConfig();
        
        return cfg;
    }
    
    private void SaveConfig()
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true, 
            };
            var json = JsonSerializer.Serialize(Cfg, options);
            File.WriteAllText(ConfigFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    public string GetSelectedUserParameter() => Cfg.SelectedUserParameter;
    public void SetSelectedUserParameter(string value) => Cfg.SelectedUserParameter = value;

}