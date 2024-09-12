using System.IO;
using System.Windows;
using FamilyCleaner.Models.Entities;
using FamilyCleaner.Models.Worker;
using Ookii.Dialogs.Wpf;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace FamilyCleaner.ViewModels;

public sealed partial class FamilyCleanerViewModel : ObservableObject
{
    
     [ObservableProperty]
     private string _folderPathFrom;
     
     [ObservableProperty]
     private string _folderPathTo;
     
     [ObservableProperty]
     private List<FileItem> _rfaFiles;
    
    [RelayCommand]
    private void SetFolderFrom()
    {
        
        var dialog = new VistaFolderBrowserDialog
        {
            Description = "Выберите папку",
            UseDescriptionForTitle = true
        };

        var result = dialog.ShowDialog();
        if (result != true) return;
        FolderPathFrom = dialog.SelectedPath;
        var files = Directory.GetFiles(FolderPathFrom, "*.rfa");

        RfaFiles = files.Select(f => new FileItem 
        {
            FileName = Path.GetFileName(f),   
            FullPath = f,                      
            IsSelected = false
        }).ToList();
    }
    
    [RelayCommand]
    private void SetFolderTo()
    {
        
        var dialog = new VistaFolderBrowserDialog
        {
            Description = "Выберите папку",
            UseDescriptionForTitle = true
        };

        var result = dialog.ShowDialog();
        if (result == true)
        {
            FolderPathTo = dialog.SelectedPath;
        }
    }

    [RelayCommand]
    private void Execute(Window window)
    {
        
        foreach (var fileItem in RfaFiles.Where(fileItem => fileItem.IsSelected))
        {
            try
            {
                var destinationPath = Path.Combine(_folderPathTo, fileItem.FileName);
                var worker = new Worker();
                worker.Execute(fileItem.FullPath, destinationPath);
            }
            catch (Exception e)
            {
                TaskDialog.Show("Error", e.ToString());
            }
        }
        window?.Close();
    }
}