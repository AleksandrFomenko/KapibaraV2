﻿using System.Windows.Controls;
using KapibaraUI.Services.Appearance;
using SortingCategories.ViewModels;
using Wpf.Ui.Abstractions.Controls;

namespace SortingCategories.Views;

public partial class MainFamilies : INavigableView<SortingCategoriesViewModel>
{
    public MainFamilies(SortingCategoriesViewModel viewModel, IThemeWatcherService themeWatcherService)
    {
        themeWatcherService.Watch(this);
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    public SortingCategoriesViewModel ViewModel { get; }
}