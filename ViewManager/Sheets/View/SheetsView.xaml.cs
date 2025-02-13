using System.Windows.Controls;

namespace ViewManager.Sheets.ViewModel;

public partial class SheetsView : UserControl
{
    public SheetsView()
    {
        var doc = Context.ActiveDocument;
        DataContext = new SheetsViewModel(doc);
        InitializeComponent();
    }
}