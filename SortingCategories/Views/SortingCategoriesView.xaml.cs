using SortingCategories.ViewModels;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace SortingCategories.Views;

public sealed partial class SortingCategoriesView : FluentWindow
{
    public SortingCategoriesView(SortingCategoriesViewModel viewModel)
    {
        ApplicationThemeManager.Apply(this);
        DataContext = viewModel;
        InitializeComponent();
    }
}