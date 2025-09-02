using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text.Json;
using ExporterModels.Dialogs.Settings.Entities;
using ExporterModels.Entities;

namespace ExporterModels.services;

public sealed class ConfigurationService
{
    private const string RootDirName = "ExportModels";
    private const string ConfigName = "config.json";
    private static readonly string DllPath = Assembly.GetExecutingAssembly().Location;
    private static readonly string DllDirectory = Path.GetDirectoryName(DllPath)!;

    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private readonly string _configFilePath;
    private readonly string _configsStoreDir;

    private readonly string _rootDir;

    public ConfigurationService()
    {
        _rootDir = Path.Combine(DllDirectory, RootDirName);
        _configFilePath = Path.Combine(_rootDir, ConfigName);
        _configsStoreDir = Path.Combine(_rootDir, "Configs");

        Directory.CreateDirectory(_rootDir);
        Directory.CreateDirectory(_configsStoreDir);

        if (!File.Exists(_configFilePath))
            File.WriteAllText(_configFilePath, JsonSerializer.Serialize(new AppSettings(), _jsonOptions));
    }

    public void Save(AppSettings settings)
    {
        File.WriteAllText(_configFilePath, JsonSerializer.Serialize(settings, _jsonOptions));
    }

    public AppSettings Load()
    {
        var text = File.ReadAllText(_configFilePath);
        return JsonSerializer.Deserialize<AppSettings>(text, _jsonOptions) ?? new AppSettings();
    }

    public string GetPath()
    {
        return _configFilePath;
    }

    public string GetConfigsStoreDir()
    {
        return _configsStoreDir;
    }

    public string BuildInternalConfigPath(string configName)
    {
        var baseName = MakeSafeFileName(string.IsNullOrWhiteSpace(configName) ? "config" : configName.Trim());
        return Path.Combine(_configsStoreDir, baseName + ".json");
    }

    public string EnsureInternalConfig(string configName)
    {
        var path = BuildInternalConfigPath(configName);
        if (!File.Exists(path)) File.WriteAllText(path, "{}");
        return path;
    }

    public WorkspaceConfig LoadWorkspace(string workspacePath)
    {
        if (string.IsNullOrWhiteSpace(workspacePath) || !File.Exists(workspacePath))
            return new WorkspaceConfig();

        var text = File.ReadAllText(workspacePath);
        return string.IsNullOrWhiteSpace(text)
            ? new WorkspaceConfig()
            : JsonSerializer.Deserialize<WorkspaceConfig>(text, _jsonOptions) ?? new WorkspaceConfig();
    }

    public void SaveWorkspace(string workspacePath, WorkspaceConfig data)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(workspacePath)!);
        File.WriteAllText(workspacePath, JsonSerializer.Serialize(data, _jsonOptions));
    }

    public void ExportConfigFile(string sourcePath, string destPath)
    {
        if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
            throw new FileNotFoundException("Исходный конфигурационный файл не найден.", sourcePath);

        Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
        File.Copy(sourcePath, destPath, true);
    }

    private static string MakeSafeFileName(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name;
    }
}

public sealed class AppSettings
{
    public ObservableCollection<Configuration> Configurations { get; set; } =
        [];

    public string? SelectedConfigurationPath { get; set; }
}

public sealed class WorkspaceConfig
{
    public ObservableCollection<Project> Projects { get; set; } =
        [];

    public string? SelectedProjectName { get; set; }
}