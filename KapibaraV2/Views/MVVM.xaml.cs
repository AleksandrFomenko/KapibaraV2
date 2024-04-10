using System.Windows;
using KapibaraV2.ViewModels;

namespace KapibaraV2.Views
{
    public partial class MVVM : Window
    {
        public MVVM(TestViewModel testViewModel)
        {
            DataContext = testViewModel;
            InitializeComponent();
        }
    }
}