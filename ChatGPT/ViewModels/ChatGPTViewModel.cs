using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Autodesk.Revit.UI;
using ChatGPT.model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;

namespace ChatGPT.ViewModels;

public partial class ChatGptViewModel : INotifyPropertyChanged
{
    private string _request;
    private string _result;

    private ChatGPTModel _model = new ChatGPTModel();
    public string Request
    {
        get => _request;
        
        set
        {
            if (_request != value)
            {
                _request = value;
                OnPropertyChanged();
            }
        }
    }
    
    public string Response => _result;
    
    [RelayCommand]
    private async Task MakeRequestAsync()
    {
        if (string.IsNullOrWhiteSpace(Request))
        {
            _result = "Запрос не может быть пустым.";
            OnPropertyChanged(nameof(Response));
            return;
        }

        try
        {
            _result = "Отправка запроса...";
            OnPropertyChanged(nameof(Response));

            string result = await _model.SendPromptAsync(Request);
            
            _result = result;
            OnPropertyChanged(nameof(Response));
        }
        catch (Exception ex)
        {
            _result = $"Произошла ошибка: {ex.Message}";
            OnPropertyChanged(nameof(Response));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
}