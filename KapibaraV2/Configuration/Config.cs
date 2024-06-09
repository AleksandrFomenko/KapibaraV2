using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.UI;


namespace KapibaraV2.Configuration
{
    public class Config
    {
        public string PathConfig { get; set; }

        private static readonly string dllPath = Assembly.GetExecutingAssembly().Location;
        private static readonly string dllDirectory = Path.GetDirectoryName(dllPath);
        private static readonly string ConfigFilePath = Path.Combine(dllDirectory, "config", "config.json");



        // Метод для записи конфигурации в файл JSON
        public void SaveConfig()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
              
            }
        }
        // Метод для чтения конфигурации из файла JSON
        public static Config LoadConfig()
        {
            try
            {
                if (!File.Exists(ConfigFilePath))
                {
                    throw new FileNotFoundException("Файл конфигурации не найден");
                }

                string json = File.ReadAllText(ConfigFilePath);
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
                return null ;
            }
        }
        // Метод для получения значения Path из конфигурации
        public static string GetConfigPath()
        {
            Config config = LoadConfig();
            return config?.PathConfig; 
        }
    }
}
