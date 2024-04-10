using System.ComponentModel;

namespace KapibaraV2.Commands.Models.mvvmTest
{
    public class Model : INotifyPropertyChanged
    {
        private double _number1;
        private double _number2;
        private double _sum;

        public double Number1
        {
            get { return _number1; }
            set
            {
                _number1 = value;
                OnPropertyChanged(nameof(Number1));
                CalculateSum();
            }
        }

        public double Number2
        {
            get { return _number2; }
            set
            {
                _number2 = value;
                OnPropertyChanged(nameof(Number2));
                CalculateSum();
            }
        }

        public double Sum
        {
            get { return _sum; }
            private set { _sum = value; OnPropertyChanged(nameof(Sum)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CalculateSum()
        {
            Sum = Number1 + Number2;
        }
    }
}
