using System.Collections.ObjectModel;
using Autodesk.Revit.UI;
using KapibaraCore.Parameters;
using KapibaraUI.Services.Appearance;
using Wpf.Ui.Appearance;

namespace SortingCategories.ViewModels;

public sealed partial class SortingCategoriesViewModel : ObservableObject
{
    private readonly Document _doc;


    [ObservableProperty] private bool _darkTheme = true;
    [ObservableProperty] private bool _isAllChecked;
    [ObservableProperty] private ObservableCollection<RevitCategory> _revitCategories;
    [ObservableProperty] private RevitCategory _revitCategory;
    [ObservableProperty] private List<string> _projectParameters;
    private readonly List<Category> _projectCategory;


    partial void OnDarkThemeChanged(bool value)
    {
        ApplicationTheme applicationTheme =
            ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Light
                ? ApplicationTheme.Dark
                : ApplicationTheme.Light;

        ThemeWatcherService.ApplyTheme(applicationTheme);
    }
    partial void OnIsAllCheckedChanged(bool value)
    {
        foreach (var item in RevitCategories)
        {
            item.IsChecked = value;
        }
    }
    public SortingCategoriesViewModel(Document doc)
    {
        RevitCategories = new ObservableCollection<RevitCategory>();
        _doc = doc;
        ProjectParameters = _doc.GetProjectParameters();
        
        var categories = _doc.Settings.Categories;
        _projectCategory = categories
            .Cast<Category>()
            .Where(i => i.IsVisibleInUI)
            .Where(i => i.AllowsBoundParameters)
            .Where(i => i.CategoryType == CategoryType.Model)
            .ToList();
    }
    
    [RelayCommand]
    private void Add() 
        =>  RevitCategories.Add(new RevitCategory() {IsChecked = true, Categories = _projectCategory});

    [RelayCommand]
    private void Delete()
        => RevitCategories.Remove(RevitCategory ?? RevitCategories.LastOrDefault());

    
    [RelayCommand]
    private void DuctPattern()
    {
        RevitCategories.Clear();
        RevitCategories = Pattern.GenerateRevitCategories(_doc, _projectCategory, 1);
    }
    
    [RelayCommand]
    private void PipelineHotPattern()
    {
        RevitCategories.Clear();
        RevitCategories = Pattern.GenerateRevitCategories(_doc, _projectCategory, 2);
    }
    
    [RelayCommand]
    private void Clear()
    {
        RevitCategories.Clear();
    }

    [RelayCommand]
    private void Execute()
    {
        

    }
}