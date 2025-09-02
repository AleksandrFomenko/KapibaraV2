﻿using ExporterModels.Dialogs.RemoveProject.ViewModel;
using KapibaraUI.Services.Appearance;

namespace ExporterModels.Dialogs.RemoveProject.View;

public partial class RemoveProjectView
{
    public RemoveProjectView(RemoveProjectViewModel viewModel, IThemeWatcherService themeWatcherService)
    {
        ViewModel = viewModel;
        DataContext = viewModel;
        RemoveProjectViewModel.CloseAction = Close;
        themeWatcherService.Watch(this);
        InitializeComponent();
    }

    public RemoveProjectViewModel ViewModel { get; }
}