using System.Collections.ObjectModel;
using ExporterModels.Dialogs.Settings.Entities;

namespace ExporterModels.Dialogs.AddConfiguration.Model;

public class AddConfigurationModel
{
    public void AddConfigurations(ObservableCollection<Configuration> configurations, string name, string? path)
    {
        configurations.Add(new Configuration(name, path));
    }
}