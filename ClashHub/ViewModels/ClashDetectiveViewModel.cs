using System.Collections.ObjectModel;
using ClashHub.Domain.Entities;
using ClashHub.Models.Entity;
using ClashHub.Models.Parsers;
using ClashHub.Models.Parsers.Xml;
using ClashHub.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;


namespace ClashHub.ViewModels;

public sealed partial class ClashDetectiveViewModel : ObservableObject
{
    private IPickerElements _picker;
    [ObservableProperty] private string _pathToFile;
    [ObservableProperty] private List<IFileParser<ClashTest>> _formats;
    [ObservableProperty] private IFileParser<ClashTest> _selectedFormat;
    [ObservableProperty] private ObservableCollection<ClashTest> _checks = [];
    [ObservableProperty] private ClashTest _selectedCheck;
    [ObservableProperty] private ObservableCollection<ClashResult> _collisions = [];
    [ObservableProperty] private ClashResult _selectedCollision;
    [ObservableProperty] private string _selectedCollisionImagePath;
    [ObservableProperty] private ElementEntity _firstElement;
    [ObservableProperty] private ElementEntity _secondElement;
    [ObservableProperty] private bool _visibleElementsInfo = true;
    
    partial void OnSelectedCheckChanged(ClashTest value)
    {
        Collisions.Clear();
        foreach (var clashResult in value.Results)
        {
            Collisions.Add(clashResult);
            clashResult.ShowEvent += ShowElement;
        }
    }
    
    partial void OnSelectedCollisionChanged(ClashResult value)
    {
        if (value == null) return;
    
        if (!string.IsNullOrEmpty(value.Href))
        {
            var dir = System.IO.Path.GetDirectoryName(PathToFile);
            var href = value.Href.Replace('\\', System.IO.Path.DirectorySeparatorChar);
            if (dir != null) SelectedCollisionImagePath = System.IO.Path.Combine(dir, href);
        }
    
        GetCollisionElements();
    }

    public ClashDetectiveViewModel(IPickerElements picker)
    {
        _picker = picker;
        Formats = [new XmlFileParser()];
        SelectedFormat = Formats.First();
    }

    [RelayCommand]
    private void SelectPath()
    {
        var dlg = new OpenFileDialog { Filter = "XML files (*.xml)|*.xml" };
        if (dlg.ShowDialog() != true) return;
        PathToFile = dlg.FileName;
        try
        {
            ParseSelectedFormat();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    [RelayCommand]
    private void SelectFirstElement()
    {
        if (FirstElement == null) return;
        PickElement(FirstElement.Id);
    }
    
    [RelayCommand]
    private void SelectSecondElement()
    {
        if (SecondElement == null) return;
        PickElement(SecondElement.Id);
    }

    private void PickElement(int id)
    {
        _picker.PickElement(id);
    }

    private void ParseSelectedFormat()
    {
        if (string.IsNullOrWhiteSpace(PathToFile)) return;

        var parsedChecks = SelectedFormat.Parse(PathToFile);
        Checks.Clear();
        foreach (var check in parsedChecks) Checks.Add(check);

        SelectedCheck = Checks.FirstOrDefault();
        SelectedCollision = SelectedCheck?.Results.FirstOrDefault();
    }

    private void GetCollisionElements()
    {
        var objects = SelectedCollision?.Objects;

        var firstObj  = objects?.ElementAtOrDefault(0);
        var secondObj = objects?.ElementAtOrDefault(1);

        var firstElementId  = GetElementId(firstObj)  ?? "0";
        var secondElementId = GetElementId(secondObj) ?? "0";

        var firstElementType       = firstObj?.SmartTags?.ElementAtOrDefault(0)?.Value ?? "error";
        var secondElementType      = secondObj?.SmartTags?.ElementAtOrDefault(0)?.Value ?? "error";

        var firstElementFamilyType  = firstObj?.Layer  ?? "error";
        var secondElementFamilyType = secondObj?.Layer ?? "error";

        if (!int.TryParse(firstElementId,  out var firstId))  firstId  = 0;
        if (!int.TryParse(secondElementId, out var secondId)) secondId = 0;

        FirstElement  = new ElementEntity(firstElementType,  firstId,  firstElementFamilyType);
        SecondElement = new ElementEntity(secondElementType, secondId, secondElementFamilyType);
    }

    private string? GetElementId(ClashObject? obj)
    {
        return obj?.SmartTags?
            .FirstOrDefault(t => t.Name?.Equals("Объект Id", StringComparison.OrdinalIgnoreCase) == true)
            ?.Value;
    }

    private void ShowElement()
    {
        Console.WriteLine("нажал");
    }
}