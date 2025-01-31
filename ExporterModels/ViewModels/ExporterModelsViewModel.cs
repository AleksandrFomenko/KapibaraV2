using ExporterModels.Models.Configuration;

namespace ExporterModels.ViewModels;

public sealed class ExporterModelsViewModel : ObservableObject
{
    internal ExporterModelsViewModel()
    {
        PathJson.CheckConfig();
    }
    
}