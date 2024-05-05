using KapibaraV2.ViewModels.MepGeneral;
using System.Windows;


namespace KapibaraV2.Views.MepGeneral
{
    public partial class FloorFillerVIew : Window
    {
        public FloorFillerVIew(FloorFillerModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

