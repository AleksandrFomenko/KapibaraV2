using KapibaraV2.Commands.Models.mvvmTest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapibaraV2.ViewModels.MepGeneral
{
    public class FloorFillerModel : INotifyPropertyChanged
    {
        private readonly Model _model;
        

        public FloorFillerModel  ()
        {
            InitializeParameters();
        }
        private void InitializeParameters() { }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
