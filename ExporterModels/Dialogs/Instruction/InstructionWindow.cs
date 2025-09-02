﻿using System.Windows;
using ExporterModels.Dialogs.Instruction.Abstraction;
using ExporterModels.Dialogs.Instruction.Service;
using ExporterModels.Dialogs.Instruction.View;
using ExporterModels.Dialogs.Instruction.ViewModel;
using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;

namespace ExporterModels.Dialogs.Instruction;

public static class InstructionWindow
{
    public static void Show(Window? owner, Action onClosed)
    {
        var services = new ServiceCollection();

        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        services.AddSingleton<IServiceInstruction, InstructionService>();

        services.AddTransient<InstructionViewModel>();
        services.AddTransient<InstructionView>();
        var serviceProvider = services.BuildServiceProvider();
        var tws = serviceProvider.GetService<IThemeWatcherService>();

        var view = serviceProvider.GetService<InstructionView>();
        view.Owner = owner;
        view.Show();
        tws.SetConfigTheme(view);
        view.Loaded += (_, __) => tws.SetConfigTheme(view);
        view.Closed += (_, _) => { onClosed?.Invoke(); };
        view.Show();
    }
}