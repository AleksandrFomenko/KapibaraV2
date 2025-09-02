using System.Collections.ObjectModel;
using ClashHub.Domain.Entities;
using ClashHub.Models.Entity;
using ClashHub.Models.Parsers;
using ClashHub.Models.Parsers.Xml;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;


namespace ClashHub.ViewModels;

public sealed partial class ClashDetectiveViewModel : ObservableObject
{
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
        }
    }
    
    partial void OnSelectedCollisionChanged(ClashResult value)
    {
        var dir = System.IO.Path.GetDirectoryName(PathToFile);
        var href = SelectedCollision.Href.Replace('\\', System.IO.Path.DirectorySeparatorChar);
        if (dir != null) SelectedCollisionImagePath = System.IO.Path.Combine(dir, href);
        GetCollisionElements();
    }

    public ClashDetectiveViewModel()
    {
        Formats =
        [
            new XmlFileParser()
        ];
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

    private void ParseSelectedFormat()
    {
        if (string.IsNullOrWhiteSpace(PathToFile))
            return;
        
        var parsedChecks = SelectedFormat.Parse(PathToFile);
        
        Checks.Clear();
        
        foreach (var check in parsedChecks)
        {
            Checks.Add(check);
        }
        
        SelectedCheck = Checks.FirstOrDefault(); 
        SelectedCollision = SelectedCheck?.Results.FirstOrDefault();
        GetCollisionElements();
    }

    private void GetCollisionElements()
    {
        var firstElementId = SelectedCollision?.Objects?[0]?.ObjectAttribute.Value ?? "0";
        var secondElementId = SelectedCollision?.Objects?[1]?.ObjectAttribute.Value ?? "0";

        var firstElementType = SelectedCollision?.Objects?[0]?.SmartTags[1].Value ?? "error";
        var secondElementType = SelectedCollision?.Objects?[1]?.SmartTags[1].Value ?? "error";

        var firstElementFamilyType = SelectedCollision?.Objects?[0]?.SmartTags[0].Value ?? "error";
        var secondElementFamilyType = SelectedCollision?.Objects?[1]?.SmartTags[0].Value ?? "error";
        
        FirstElement = new ElementEntity(firstElementType, int.Parse(firstElementId), firstElementFamilyType);
        SecondElement = new ElementEntity(secondElementType, int.Parse(secondElementId), secondElementFamilyType);
    }
}