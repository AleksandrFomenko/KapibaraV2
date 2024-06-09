using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KapibaraV2.ViewModels.BIM;
using KapibaraV2.Configuration;

namespace KapibaraV2.Views.BIM
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class ExportModelsView : Window
    {
        public ExportModelsView(ExportModelsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string configPath = Config.GetConfigPath();
            Autodesk.Revit.UI.TaskDialog.Show("Succeeded", configPath);

        }
    }
}
