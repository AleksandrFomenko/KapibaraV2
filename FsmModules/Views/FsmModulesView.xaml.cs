using System.Windows;
using System.Windows.Input;
using FsmModules.ViewModels;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace FsmModules.Views;

public sealed partial class FsmModulesView
{
    public FsmModulesView(FsmModulesViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
        InitializeMaterialDesign();
    }
    private void InitializeMaterialDesign()
    {
        var card = new Card();
        var hue = new Hue("Dummy", Colors.Black, Colors.White);
    }
    private void CloseIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        this.Close();
    }
    private void FullscreenIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        ToggleFullscreen();
    }
    
    private void MinimizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }
    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }
    
    private void ToggleFullscreen()
    {
        if (this.WindowState == WindowState.Normal)
        {
            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
        }
        else
        {
            this.WindowState = WindowState.Normal;
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.ResizeMode = ResizeMode.CanResize;
        }
    }
    private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            ToggleFullscreen();
        }
        else
        {
            this.DragMove();
        }
    }
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Escape)
        {
            this.Close();
        }
    }
}