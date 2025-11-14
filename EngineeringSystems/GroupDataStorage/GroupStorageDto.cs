using Group = EngineeringSystems.ViewModels.Entities.Group;

namespace EngineeringSystems.GroupDataStorage;

public class GroupStorageDto
{
    public List<Group> Data { get; set; } = [];
}