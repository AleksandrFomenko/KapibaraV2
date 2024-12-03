using System.ComponentModel;
using System.Runtime.CompilerServices;
using Color = System.Windows.Media.Color;

namespace ColorsByParameters.ViewModels;

public class TextColorPair : INotifyPropertyChanged
{
    private string _text;
    private Color _color;
    
    public string Text
    {
        get => _text;
        set
        {
            if (_text != value)
            {
                _text = value;
                OnPropertyChanged();
            }
        }
    }


    public System.Windows.Media.Color Color
    {
        get => _color;
        set
        {
            if (_color != value)
            {
                _color = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}