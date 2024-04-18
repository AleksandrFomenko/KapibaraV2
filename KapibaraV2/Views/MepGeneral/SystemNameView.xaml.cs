using System.Windows;
using KapibaraV2.ViewModels.MepGeneral;

namespace KapibaraV2.Views.MepGeneral
{
    public partial class SystemNameView : Window
    {
        public SystemNameView(SystemNameViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            if (viewModel.CloseAction == null)
                viewModel.CloseAction = new Action(this.Close);


        }
    }
}
