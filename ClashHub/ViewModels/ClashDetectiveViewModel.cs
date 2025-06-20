using System.Collections.ObjectModel;
using ClashHub.Models.Entity;
using ClashHub.Models.Parsers.Xml;
using ClashHub.Structure;
using ClashHub.ViewModels.Format;
using Microsoft.Win32;

namespace ClashHub.ViewModels;

public sealed partial class ClashDetectiveViewModel : ObservableObject
{
    [ObservableProperty] private string _pathToFile;
    [ObservableProperty] private List<IFormat<Clashtest>> _formats;
    [ObservableProperty] private IFormat<Clashtest> _selectedFormat;
    [ObservableProperty] private ObservableCollection<Clashtest> _checks = [];
    [ObservableProperty] private Clashtest _selectedCheck;
    [ObservableProperty] private ObservableCollection<ClashResult> _collisions = [];
    [ObservableProperty] private ClashResult _selectedCollision;
    [ObservableProperty] private string _selectedCollisionImagePath;
    [ObservableProperty] private ElementEntity _firstElement;
    [ObservableProperty] private ElementEntity _secondElement;
    [ObservableProperty] private bool _visibleElementsInfo = true;
  
    
    partial void OnSelectedCheckChanged(Clashtest value)
    {
        Collisions.Clear();
        foreach (var clashResult in value.ClashResults.Results)
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
            new Format<XmlStructure, Clashtest>(
                "XML",
                new XmlFileParser<XmlStructure>(),
                xml => xml.Batchtest.Clashtests.ClashTestList
            )
        ];
        SelectedFormat = Formats.First();
    }

    [RelayCommand]
    private void SelectPath()
    {
        var dlg = new OpenFileDialog { Filter = "XML files (*.xml)|*.xml" };
        if (dlg.ShowDialog() != true) return;
        PathToFile = dlg.FileName;
        ParseSelectedFormat();
        
    }

    private void ParseSelectedFormat()
    {
        if (string.IsNullOrWhiteSpace(PathToFile))
            return;
        Checks.Clear();
        SelectedFormat.Parse(PathToFile).ToList().ForEach(clashtest => Checks.Add(clashtest));
        SelectedCheck = Checks?.FirstOrDefault() ?? new Clashtest(); 
        SelectedCollision = Collisions.FirstOrDefault() ?? new ClashResult();
        GetCollisionElements();
    }

    private void GetCollisionElements()
    {
        
        var firstElementId = SelectedCollision?.ClashObjects?.Items[0]?.ObjectAttribute?.Value ?? "0";
        var secondElementId = SelectedCollision?.ClashObjects?.Items[1]?.ObjectAttribute?.Value ?? "0";

        var firstElementType = SelectedCollision?.ClashObjects?.Items[0]?.SmartTags[0]?.Value ?? "0";
        var secondElementType = SelectedCollision?.ClashObjects?.Items[1]?.SmartTags[0]?.Value ?? "0";

        var firstElementFamilyType = SelectedCollision?.ClashObjects?.Items[0]?.SmartTags[1]?.Value ?? "0";
        var secondElementFamilyType = SelectedCollision?.ClashObjects?.Items[1]?.SmartTags[1]?.Value ?? "0";
        
        FirstElement = new ElementEntity(firstElementType, int.Parse(firstElementId), firstElementFamilyType);
        SecondElement = new ElementEntity(secondElementType, int.Parse(secondElementId), secondElementFamilyType);
    }
}