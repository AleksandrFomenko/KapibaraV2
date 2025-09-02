﻿using ExporterModels.ViewModels;
using KapibaraUI.Services.Appearance;

namespace ExporterModels.Views;

public sealed partial class ExporterModelsView
{
    public ExporterModelsView(ExporterModelsViewModel viewModel, IThemeWatcherService themeWatcherService)
    {
        themeWatcherService.Watch(this);
        DataContext = viewModel;
        InitializeComponent();
    }
}