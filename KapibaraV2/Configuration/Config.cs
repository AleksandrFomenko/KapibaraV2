﻿using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using Autodesk.Revit.UI;
using KapibaraV2.Models.BIM.ExportModels;

namespace KapibaraV2.Configuration
{
    public class Config
    {
        public string PathConfig { get; set; }
        public List<Project> Projects { get; set; } = new List<Project>();
        public string badWorksetName {  get; set; } 
        
        private static readonly string DllPath = Assembly.GetExecutingAssembly().Location;
        private static readonly string DllDirectory = Path.GetDirectoryName(DllPath);
        private static readonly string ConfigFilePath = Path.Combine(DllDirectory, "config", "config.json");


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
        
        public static void UpdateBadWorkSetName(string newBadName)
        {
            var config = LoadConfig(ConfigFilePath) ?? new Config();
            config.badWorksetName = newBadName;
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

        public static void SaveProject(Project updatedProject)
        {
            var configPath = GetConfigPath();
            var config = LoadConfig(configPath);

            if (config != null)
            {
                var project = config.Projects.FirstOrDefault(p => p.Name == updatedProject.Name);
                if (project != null)
                {
                    project.Paths = updatedProject.Paths;
                    project.SavePath = updatedProject.SavePath;
                    project.badNameWorkset = updatedProject.badNameWorkset;
                    project.IfcConfigPath = updatedProject.IfcConfigPath;
                    config.SaveConfig(configPath);
                }
            }
        }
    }
}
