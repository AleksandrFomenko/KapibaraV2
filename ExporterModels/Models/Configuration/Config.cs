using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using Autodesk.Revit.UI;
using ExporterModels.Models.Entities;


namespace ExporterModels.Models.Configuration
{
    public class Config
    {
        public string PathConfig { get; set; }
        
        public List<Project> Projects { get; set; } = new List<Project>();
        
        public string BadWorksetName { get; set; }

        private static readonly string DllPath = Assembly.GetExecutingAssembly().Location;
        private static readonly string DllDirectory = Path.GetDirectoryName(DllPath);
        private static readonly string ConfigFilePath = Path.Combine(DllDirectory, "config", "config.json");
        

        public void SaveConfig(string filePath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", $"An error occurred while saving the configuration: {ex.Message}");
            }
        }
        
        public static Config LoadConfig(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("Файл конфигурации не найден", filePath);
                }

                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<Config>(json);
            }
            catch (FileNotFoundException ex)
            {
                TaskDialog.Show("File Not Found", ex.Message);
                return null;
            }
            catch (JsonException ex)
            {
                TaskDialog.Show("JSON Error", $"An error occurred while deserializing the JSON: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", $"An unexpected error occurred: {ex.Message}");
                return null;
            }
        }
        
        public static string GetConfigPath()
        {
            return LoadConfig(ConfigFilePath)?.PathConfig;
        }
        
        public static void UpdateConfigPath(string newPath)
        {
            var config = LoadConfig(ConfigFilePath) ?? new Config();
            config.PathConfig = newPath;
            config.SaveConfig(ConfigFilePath);
        }
        
        public static void UpdateBadWorkSetName(string newBadName)
        {
            var config = LoadConfig(ConfigFilePath) ?? new Config();
            config.BadWorksetName = newBadName;
            config.SaveConfig(ConfigFilePath);
        }

        public static IReadOnlyList<Project> GetProjects()
        {
            var config = LoadConfig(GetConfigPath());
            return config?.Projects.AsReadOnly() ?? new List<Project>().AsReadOnly();
        }


        public static void AddProject(Project newProject)
        {
            var configPath = GetConfigPath();
            var config = LoadConfig(configPath) ?? new Config();

            config.Projects.Add(newProject);
            config.SaveConfig(configPath);
        }
        
        public static void RemoveProject(string projectName)
        {
            var configPath = GetConfigPath();
            var config = LoadConfig(configPath);

            if (config != null)
            {
                var projectToRemove = config.Projects.FirstOrDefault(p => p.Name == projectName);
                if (projectToRemove != null)
                {
                    config.Projects.Remove(projectToRemove);
                    config.SaveConfig(configPath);
                }
            }
        }
        
        public static void SaveProject(Project updatedProject)
        {
            var configPath = GetConfigPath();
            var config = LoadConfig(configPath);

            if (config != null)
            {
                var project = config.Projects.FirstOrDefault(p => p.Name == updatedProject.Name);
                if (project != null)
                {
                    project.Models = updatedProject.Models;
                    project.SavePath = updatedProject.SavePath;
                    config.SaveConfig(configPath);
                }
            }
        }
        
    }

    internal class PathJson
    {
        public string PathStr { get; set; }
        private static readonly string DllPath = Assembly.GetExecutingAssembly().Location;
        private static readonly string DllDirectory = Path.GetDirectoryName(DllPath);
        private static readonly string ConfigFilePath = Path.Combine(DllDirectory, "config", "config.json");

        //public static string GetConfigPath()
       // {
           // return LoadConfig(ConfigFilePath)?.PathConfig;
       // }
        public static void CheckConfig()
        {
            Debug.Write(DllPath);
            Debug.Write(DllDirectory);
            try
            {
                if (!File.Exists(ConfigFilePath))
                {
                    throw new FileNotFoundException("Файл конфигурации не найден", ConfigFilePath);
                }

                var json = File.ReadAllText(ConfigFilePath);
                var x = JsonConvert.DeserializeObject<PathJson>(json);

                if (x == null || string.IsNullOrEmpty(x.PathStr))
                {
                    var pathJson = new PathJson()
                    {
                        PathStr = Path.Combine(DllDirectory, "config", "Projects.json")
                        
                    };
                    
                    json = JsonConvert.SerializeObject(pathJson, Formatting.Indented);
                    File.WriteAllText(ConfigFilePath, json);
                }
            }
            catch (FileNotFoundException ex)
            {
                TaskDialog.Show("File Not Found", ex.Message);
            }
            catch (JsonException ex)
            {
                TaskDialog.Show("JSON Error", $"An error occurred while deserializing the JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}