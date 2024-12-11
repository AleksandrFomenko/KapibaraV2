using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ViewManager.Sheets.Tabs.CreateSheets.View;

public partial class CreateSheetsView : UserControl
{
    public CreateSheetsView()
    {
        InitializeComponent();
    }
    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }
}