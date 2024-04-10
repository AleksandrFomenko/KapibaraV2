using KapibaraV2.ViewModels;

namespace KapibaraV2.Views
{
    public sealed partial class KapibaraV2View
    {
        public KapibaraV2View(KapibaraV2ViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}