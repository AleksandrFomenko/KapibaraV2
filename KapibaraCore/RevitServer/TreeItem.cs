using System.Collections.ObjectModel;

namespace KapibaraCore.RevitServer;

public class TreeItem
{
    public string Name { get; set; }
    public string Tag { get; set; }
    public ObservableCollection<TreeItem> Children { get; set; }

    public TreeItem()
    {
        Children = new ObservableCollection<TreeItem>();
    }
    
}