using System.Collections.ObjectModel;
using System.ComponentModel;
using EngineeringSystems.Dialogs.AddAndRenameGroup;
using EngineeringSystems.Dialogs.AddAndRenameGroup.EventArgs;
using EngineeringSystems.Model.Abstractions;
using EngineeringSystems.ViewModels.Entities;
using Group = EngineeringSystems.ViewModels.Entities.Group;

namespace EngineeringSystems.ViewModels;

public sealed partial class GroupSystemsViewModel : ObservableObject
{
    private readonly IGroupSystemsModel _model;
    
    [ObservableProperty]
    private ObservableCollection<Group> _systemGroups;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RenameGroupCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteGroupCommand))]
    private Group? _selectedSystemGroups;

    [ObservableProperty] 
    private ObservableCollection<EngineeringSystem> _engineeringSystems;
    [ObservableProperty] 
    private string? _filterGroupSystems;
    [ObservableProperty] 
    private string? _filterSystems;
    [ObservableProperty]
    private GroupSystemsOptions _selectedOption;
    [ObservableProperty]
    private ObservableCollection<GroupSystemsOptions> _groupSystemsOptions;

    public GroupSystemsViewModel(IGroupSystemsModel model)
    { 
        _model = model; 
        SystemGroups = _model.GetGroupSystems();
        EngineeringSystems = _model.GetProjectSystems();
        
        GroupSystemsOptions = new ObservableCollection<GroupSystemsOptions>(
            Enum.GetValues(typeof(GroupSystemsOptions))
                .Cast<GroupSystemsOptions>()
        );

        SelectedOption = GroupSystemsOptions.First();
    }

    partial void OnFilterGroupSystemsChanged(string? value)
    {
        if(value == null) return;
        SystemGroups = new ObservableCollection<Group>(
            _model.GetGroupSystems()
                .Where(x => x.Name.Contains(value, StringComparison.OrdinalIgnoreCase))
        );
    }
    
    partial void OnFilterSystemsChanged(string? value)
    {
        if(value == null) return;
        EngineeringSystems = new ObservableCollection<EngineeringSystem>(
            _model.GetProjectSystems()
                .Where(x => x.NameSystem.Contains(value, StringComparison.OrdinalIgnoreCase))
        );
    }
    
    public void HandleGroupNameRequested(object? sender, GroupNameEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.Name))
        {
            e.Result = GroupNameDialogResult.Cancelled;
            return;
        }

        var exists = SystemGroups.Any(g =>
            string.Equals(g.Name, e.Name, StringComparison.OrdinalIgnoreCase));

        if (exists)
        {
            e.Result = GroupNameDialogResult.NameIsExist;
            return;
        }

        SystemGroups.Add(new Group(e.Name, []));
        
        if (e.Name.Length <= 3)
            e.Result = GroupNameDialogResult.TooShort; 
        else
            e.Result = GroupNameDialogResult.Success;
    }
    
    public void HandleGroupRenameRequested(object? sender, GroupNameEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.Name))
        {
            e.Result = GroupNameDialogResult.Cancelled;
            return;
        }

        var name = e.Name.Trim();

        var exists = SystemGroups.Any(g =>
            string.Equals(g.Name, name, StringComparison.OrdinalIgnoreCase));

        if (exists)
        {
            e.Result = GroupNameDialogResult.NameIsExist;
            return;
        }
        
        var selected = SelectedSystemGroups;

        if (selected != null) selected.Name = name;

        if (name.Length <= 3)
            e.Result = GroupNameDialogResult.TooShort;
        else
            e.Result = GroupNameDialogResult.Success;
    }
    
    [RelayCommand] private void CheckAll() => SystemGroups.ToList().ForEach(s => s.IsChecked = true);
    [RelayCommand] private void UnCheckAll() => SystemGroups.ToList().ForEach(s => s.IsChecked = false);
    [RelayCommand] private void AddGroup() => AddAndRenameGroup.Start(Variant.Add, this);

    [RelayCommand(CanExecute = nameof(CanAddRenameGroup))]
    private void RenameGroup() => AddAndRenameGroup.Start(Variant.Rename, this);

    [RelayCommand(CanExecute = nameof(CanAddRenameGroup))]
    private void DeleteGroup()
    {
        if (SelectedSystemGroups != null) SystemGroups.Remove(SelectedSystemGroups);
    }
    private bool CanAddRenameGroup() => SelectedSystemGroups != null;

}


public enum GroupSystemsOptions
{
    [Description("Обработать все группы")] All,
    [Description("Обработать выбранные группы")] Selected,
}