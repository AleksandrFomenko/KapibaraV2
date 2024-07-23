using SolidIntersector.Models;

namespace SolidIntersector.ViewModels;

public partial class SolidIntersectorViewModel : ObservableObject
{
    [ObservableProperty]
    private string value;

    [ObservableProperty]
    private List<SelectedItems> selectedItemsList;

    [ObservableProperty]
    private List<string> parameters = new();

    [ObservableProperty]
    private string selectedParameter;

    public SolidIntersectorViewModel()
    {
        WinLoaded();
    }

    private void WinLoaded()
    {
        BindingMap bindingMap = Context.Document.ParameterBindings;
        DefinitionBindingMapIterator iterator = bindingMap.ForwardIterator();

        while (iterator.MoveNext())
        {
            Definition definition = iterator.Key;
            if (definition != null)
#if REVIT2023_OR_GREATER
            {
            ParameterType paramType = definition.StorageType;

                if (paramType == StorageType.Text ||
                    paramType == StorageType.Integer ||
                    paramType == StorageType.Number)
                {
                    Parameters.Add(definition.Name);
                }
            }
#else
            {
                ParameterType paramType = definition.ParameterType;

                if (paramType == ParameterType.Text ||
                    paramType == ParameterType.Integer ||
                    paramType == ParameterType.Number)
                {
                    Parameters.Add(definition.Name);
                }
            }
#endif
            foreach (var par in Parameters.Where(par => !Parameters.Contains(par)))
            {
                Parameters.Add(par);
            }

            if (!Parameters.Any())
            {
                Parameters = new List<string>();
            }
        }
    }
}
