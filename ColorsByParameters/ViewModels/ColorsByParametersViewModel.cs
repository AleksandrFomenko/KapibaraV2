using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Autodesk.Revit.UI;

namespace ColorsByParameters.ViewModels;

public sealed class ColorsByParametersViewModel : ObservableObject
{
    public RelayCommand StartCommand { get; }
    public RelayCommand AddConditionCommand { get; }
    public RelayCommand<string> RemoveConditionCommand { get; }

    internal ColorsByParametersViewModel()
    {
        StartCommand = new RelayCommand(
            execute: _ => Execute(),
            canExecute: _ => CanExecute()
        );

        AddConditionCommand = new RelayCommand(
            execute: _ => AddCondition(),
            canExecute: _ => true
        );

        RemoveConditionCommand = new RelayCommand<string>(
            execute: condition => Conditions.Remove(condition),
            canExecute: condition => !string.IsNullOrEmpty(condition)
        );
    }

    public ObservableCollection<string> Parameters { get; } = new ObservableCollection<string>()
    {
        "Правый нижний",
        "Правый верхний",
        "Левый нижний",
        "Левый верхний"
    };
    
    public ObservableCollection<string> FirstComboBoxItems { get; } = new ObservableCollection<string>()
    {
        "1",
        "2",
        "3"
    };
    
    public ObservableCollection<string> SecondComboBoxItems { get; } = new ObservableCollection<string>()
    {
        "4",
        "5",
        "6"
    };
    
    public ObservableCollection<string> Conditions { get; } = new ObservableCollection<string>();
    
    private string _parameter;
    public string Parameter
    {
        get => _parameter;
        set
        {
            if (_parameter != value)
            {
                _parameter = value;
                OnPropertyChanged();
            }
        }
    }
    
    private bool CanExecute()
    {
        return true;
    }

    private void Execute()
    {
        TaskDialog.Show("1б", _parameter.ToString());
    }
    
    private void AddCondition()
    {
        Conditions.Add(string.Empty);
    }
    
}