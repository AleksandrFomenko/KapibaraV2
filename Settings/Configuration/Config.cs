using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Settings.Models;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using KapibaraConfig = KapibaraCore.Configuration;

namespace Settings.Configuration;

public class Config
{
    private const string DirectoryName = "SettingsConfig";
    private const string ConfigName = "config.json";

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public UiConfig Setting { get; set; } = new UiConfig(ApplicationTheme.Light, WindowBackdropType.Mica);

    private string GetConfigPath()
    {
        var dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        var dllDir = Path.GetDirectoryName(dllPath)!;
        return Path.Combine(dllDir, DirectoryName, ConfigName);
    }

    public Config()
    {
        var dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        KapibaraConfig.Configuration.CreateDir(dllPath, DirectoryName);

        var path = GetConfigPath();

        if (!File.Exists(path))
        {
            KapibaraConfig.Configuration.CreateEmptyJsonFile(Path.GetDirectoryName(path)!, ConfigName);
            SaveConfig();
            return;
        }

        try
        {
            var json = File.ReadAllText(path);
            var loaded = JsonSerializer.Deserialize<UiConfig>(json, _jsonOptions);
            Setting = loaded ?? new UiConfig(ApplicationTheme.Light, WindowBackdropType.Mica);
        }
        catch
        {
            Setting = new UiConfig(ApplicationTheme.Light, WindowBackdropType.Mica);
            SaveConfig();
        }
    }

    public string GetPath() => GetConfigPath();

    public void SaveConfig()
    {
        try
        {
            var path = GetConfigPath();
            var json = JsonSerializer.Serialize(Setting, _jsonOptions);
            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка с сохранением конфигурации: {ex.Message}");
        }
    }
}
