using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;


namespace WpfResources;

public partial class TopPanel : UserControl
{
    public TopPanel()
    {
        InitializeComponent();
    }
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            "Heading",
            typeof(string),
            typeof(TopPanel),
            new PropertyMetadata(string.Empty));
    
    public string Heading
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }
    
    private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            var parentWindow = Window.GetWindow(this);
            parentWindow?.DragMove();
        }
    }
    
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        var parentWindow = Window.GetWindow(this);
        parentWindow?.Close();
    }
}