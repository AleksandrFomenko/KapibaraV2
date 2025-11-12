using System.Collections.ObjectModel;
using EngineeringSystems.Model.Abstractions;
using EngineeringSystems.ViewModels;
using EngineeringSystems.ViewModels.Entities;
using Group = EngineeringSystems.ViewModels.Entities.Group;

namespace EngineeringSystems.Model;

public class GroupSystemsModel : IGroupSystemsModel
{
    public ObservableCollection<Group> GetGroupSystems()
    {
        throw new NotImplementedException();
    }

    public ObservableCollection<EngineeringSystem> GetProjectSystems()
    {
        throw new NotImplementedException();
    }
}