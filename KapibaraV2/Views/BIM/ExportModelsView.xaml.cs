using System.Windows;
using KapibaraV2.ViewModels.BIM;


namespace KapibaraV2.Views.BIM
{
    public partial class ExportModelsView : Window
    {
        public ExportModelsView(ExportModelsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
