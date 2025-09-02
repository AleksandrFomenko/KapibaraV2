using System.Collections.ObjectModel;
using ExporterModels.Entities;

namespace ExporterModels.Abstractions;

public interface IExporterModel
{
    public ObservableCollection<Project> GetProjects();
}