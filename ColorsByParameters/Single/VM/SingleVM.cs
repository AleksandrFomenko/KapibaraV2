using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using ColorsByParameters.ViewModels;

namespace ColorsByParameters.Single.VM
{
    public class SingleVM : INotifyPropertyChanged
    {
        private ObservableCollection<TextColorPair> _pairs;


        public ObservableCollection<TextColorPair> Pairs
        {
            get => _pairs;
            set
            {
                if (_pairs != value)
                {
                    _pairs = value;
                    OnPropertyChanged();
                }
            }
        }

        public SingleVM()
        {
            Pairs = new ObservableCollection<TextColorPair>()
            {
                new TextColorPair { Text = "Пример 1", Color = Colors.Red },
                new TextColorPair { Text = "Пример 2", Color = Colors.Green },
                new TextColorPair { Text = "Пример 3", Color = Colors.Blue }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}