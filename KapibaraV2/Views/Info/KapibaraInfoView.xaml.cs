using KapibaraV2.ViewModels;
using System.Diagnostics;

namespace KapibaraV2.Views.Info
{
    public sealed partial class KapibaraInfoView
    {
        public KapibaraInfoView(KapibaraV2ViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void ButtonTelegram(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start("https://t.me/AleksandrFomenko228");
            this.Close();
        }

        private void ButtonGitHub(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start("https://github.com/AleksandrFomenko/KapibaraV2");
            this.Close();

        }
    }
}