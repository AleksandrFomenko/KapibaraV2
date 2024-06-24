using System.Windows;
using KapibaraV2.ViewModels.Tools;

namespace KapibaraV2.Views.Info
{
    public partial class ToolsKapiView : Window
    {
        public ToolsKapiView(ToolsKapiViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            this.Closing += ToolsKapiView_Closing;
        }

        private void ToolsKapiView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is ToolsKapiViewModel viewModel)
            {
                viewModel.OnWindowClosing();
            }
        }
    }
}
