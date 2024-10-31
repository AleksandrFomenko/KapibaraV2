using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ViewManager.Models.Entites;

public class ViewSheetItem : INotifyPropertyChanged
{
    private bool _isChecked;
    private string _number;
    private string _name;
    private int _id; 

    public bool IsChecked
    {
        get => _isChecked;
        set { _isChecked = value; OnPropertyChanged(); }
    }

    public string Number
    {
        get => _number;
        set { _number = value; OnPropertyChanged(); }
    }

    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    public int ID
    {
        get => _id;
        set { _id = value; OnPropertyChanged(); }
    }
    

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}