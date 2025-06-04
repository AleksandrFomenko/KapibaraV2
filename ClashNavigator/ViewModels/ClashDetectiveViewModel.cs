using System.Collections.ObjectModel;
using ClashDetective.Models.Parsers.Xml;
using ClashDetective.Structure;
using ClashDetective.ViewModels.Format;
using Microsoft.Win32;

namespace ClashDetective.ViewModels;

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
        foreach (var clashtest in SelectedFormat.Parse(PathToFile))
            Checks.Add(clashtest);
        
        SelectedCheck = Checks?.FirstOrDefault();
        SelectedCollision = Collisions.FirstOrDefault();
    }
}