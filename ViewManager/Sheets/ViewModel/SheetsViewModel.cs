using System.ComponentModel;
using System.Runtime.CompilerServices;
using ViewManager.Sheets.Tabs.CreateSheets.VM;
using ViewManager.Sheets.Tabs.Print.Model;
using ViewManager.Sheets.Tabs.Print.VM;


namespace ViewManager.Sheets.ViewModel;

internal class SheetsViewModel: INotifyPropertyChanged
{
    private Document _doc;
    public string Header => "Менеджер листов";
    public CreateSheetsVM CreateSheetsViewModel { get; }
    public PrintVm PrintViewModel { get; }


    public SheetsViewModel(Document doc)
    {
        _doc = doc;
        var printModel = new PrintModel(_doc); 
        CreateSheetsViewModel = new CreateSheetsVM(_doc);
        
        PrintViewModel = new PrintVm(_doc, printModel);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
}