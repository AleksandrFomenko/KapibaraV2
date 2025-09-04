using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;
using WorkSetLinkFiles.Models;
using WorkSetLinkFiles.ViewModels;
using WorkSetLinkFiles.Views;

namespace WorkSetLinkFiles.Commands;

public static class Host
{
   public static void Start()
   {
      var services = new ServiceCollection();
      var doc = Context.ActiveDocument;
      if (doc == null)  return;

      services.AddSingleton(doc); 
      services.AddSingleton<WorkSetLinkFilesViewModel>();
      services.AddSingleton<WorkSetLinkFilesModel>();
      services.AddSingleton<WorkSetLinkFilesView>();
      services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
      var serviceProvider = services.BuildServiceProvider();
      var view = serviceProvider.GetRequiredService<WorkSetLinkFilesView>();
      var tws = serviceProvider.GetRequiredService<IThemeWatcherService>();
      view.SourceInitialized += (sender, args) => tws.SetConfigTheme();
      view.ShowDialog();
   }
}