using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autodesk.Revit.UI;
using KapibaraV2.Models.BIM.ExportModels;
using Autodesk.Revit.DB;

namespace KapibaraV2.Configuration
{
    public class Config
    {
        public string PathConfig { get; set; }
        public List<Project> Projects { get; set; } = new List<Project>();

        private static readonly string dllPath = Assembly.GetExecutingAssembly().Location;
        private static readonly string dllDirectory = Path.GetDirectoryName(dllPath);
        private static readonly string ConfigFilePath = Path.Combine(dllDirectory, "config", "config.json");

        public void SaveConfig(string filePath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
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
                    throw new FileNotFoundException("Файл конфигурации не найден");
                }

                string json = File.ReadAllText(filePath);
                Config config = JsonConvert.DeserializeObject<Config>(json);
                return config;
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
            Config config = LoadConfig(ConfigFilePath);
            return config?.PathConfig;
        }

        public static void UpdateConfigPath(string newPath)
        {
            var config = LoadConfig(ConfigFilePath) ?? new Config();
            config.PathConfig = newPath;
            config.SaveConfig(ConfigFilePath);
        }

        public static List<Project> GetProjects()
        {
            var config = LoadConfig(GetConfigPath());
            return config?.Projects ?? new List<Project>();
        }

        public static void AddProject(Project newProject)
        {
            var configPath = GetConfigPath();
            var config = LoadConfig(configPath);

            if (config == null)
            {
                config = new Config();
            }

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
    }
}
