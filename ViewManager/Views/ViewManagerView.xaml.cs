using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using ViewManager.Legends.ViewModel;
using ViewManager.ViewModels;

namespace ViewManager.Views;

public sealed partial class ViewManagerView
{
    public ViewManagerView(ViewManagerViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeMaterialDesign();
        InitializeComponent();
        ViewManagerViewModel.CloseWindow = this.Close;
    }
    
    private void InitializeMaterialDesign()
    {
        var card = new Card();
        var hue = new Hue("Dummy", Colors.Black, Colors.White);
    }

}