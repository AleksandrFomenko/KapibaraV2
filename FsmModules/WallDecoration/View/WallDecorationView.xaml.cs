using System.Windows.Controls;
using System.Windows.Media;
using FsmModules.WallDecoration.ViewModel;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace FsmModules.WallDecoration.View;

public partial class WallDecorationView : UserControl
{
    public WallDecorationView()
    {
        DataContext = new WallDecorationViewModel(Context.ActiveDocument);
        InitializeComponent();
    }
}