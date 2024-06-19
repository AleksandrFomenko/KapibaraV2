using KapibaraV2.ViewModels.BIM.AddDeleteProjects;
using KapibaraV2.ViewModels.BIM.ExportModels;
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
using System.Windows.Shapes;

namespace KapibaraV2.Views.BIM.ExportModels
{
    public partial class AddModelsView : Window
    {
        public AddModelsView(AddModelsViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
