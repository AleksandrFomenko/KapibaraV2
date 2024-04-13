using KapibaraV2.Commands.Models.mvvmTest;
using System.ComponentModel;

namespace KapibaraV2.ViewModels
{
    public class TestViewModel : INotifyPropertyChanged
    {
        private readonly Model _model;

        public TestViewModel(Model model)
        {
            _model = model;
            _model.PropertyChanged += (sender, e) => { OnPropertyChanged(e.PropertyName); };
        }

        public double Number1
        {
            get => _model.Number1;
            set => _model.Number1 = value;
        }

        public double Number2
        {
            get => _model.Number2;
            set => _model.Number2 = value;
        }

        public double Sum => _model.Sum;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
