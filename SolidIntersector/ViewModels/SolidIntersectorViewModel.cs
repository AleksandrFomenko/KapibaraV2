using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using SolidIntersector.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autodesk.Revit.UI;

namespace SolidIntersector.ViewModels;


public partial class SolidIntersectorViewModel : ObservableObject
{
    [ObservableProperty]
    private string value;

    [ObservableProperty]
    private List<SelectedItems> itemsList= new ();
    
    [ObservableProperty]
    private SelectedItems selectedItem= new ();

    [ObservableProperty]
    private List<string> parameters = new();

    [ObservableProperty]
    private string selectedParameter;

    public SolidIntersectorViewModel()
    {
        LoadedParameters();
        LoadedFamilies();
    }

    private void LoadedParameters()
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

    private void LoadedFamilies()
    {
        var families = new FilteredElementCollector(Context.Document)
            .OfCategory(BuiltInCategory.OST_GenericModel)
            .WhereElementIsNotElementType()
            .ToElements()
            .ToList();
        ItemsList = families
            .Select(f => new SelectedItems
            {
                NameItem = f.Name,
                IsChecked = false
            })
            .ToList();
    }
    private List<Element> FindIntersectingElementsBoundingBox(string elementName)
    {
        var element = new FilteredElementCollector(Context.Document)
            .OfCategory(BuiltInCategory.OST_GenericModel)
            .WhereElementIsNotElementType()
            .FirstOrDefault(e => e.Name.Equals(elementName));

        if (element == null)
        {
            TaskDialog.Show("Ошибка", $"Элемент '{elementName}' не найден.");
            return new List<Element>();
        }

        var boundingBox = element.get_BoundingBox(null);
        
        Outline outline = new Outline(boundingBox.Min, boundingBox.Max);
        BoundingBoxIntersectsFilter filter = new BoundingBoxIntersectsFilter(outline);

        var elements = new FilteredElementCollector(Context.Document)
            .WherePasses(filter)
            .WhereElementIsNotElementType()
            .ToList();

        return elements;
    }
    [RelayCommand]
    private void Execute(Window window)
    {
        var selectedItems = ItemsList.FirstOrDefault(item => item.IsChecked);
        
        if (selectedItems == null)
        {
            TaskDialog.Show("Ошибка", "Выберите элемент.");
        }
    }
}
