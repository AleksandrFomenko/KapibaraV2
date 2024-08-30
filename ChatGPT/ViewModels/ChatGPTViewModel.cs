using System.Windows;
using Autodesk.Revit.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.ViewModels;

public partial class ChatGptViewModel : ObservableObject
{
    [ObservableProperty] private string request;

    [ObservableProperty] private string response;
    
    
    [RelayCommand]
    private void MakeRequest(Window window)
    {
        response = "ав";
    }
}