﻿using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using SolidIntersector.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autodesk.Revit.UI;
using CommunityToolkit.Mvvm.Input;
using Helper.Models;
using Nice3point.Revit.Toolkit;


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
    private List<Element> FindIntersectingElements(string elementName)
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

        // Фильтр с BB
        var boundingBox = element.get_BoundingBox(null);
        Outline outline = new Outline(boundingBox.Min, boundingBox.Max);
        BoundingBoxIntersectsFilter filter = new BoundingBoxIntersectsFilter(outline);

        // Фильтр с солидом
        Solid solid = GetSolid(element);
        ElementIntersectsSolidFilter solidFilter = new ElementIntersectsSolidFilter(solid);
        
        var elements = new FilteredElementCollector(Context.Document)
            .WherePasses(filter)
            .WherePasses(solidFilter)
            .WhereElementIsNotElementType()
            .ToList();

        return elements;
    }
    [RelayCommand]
    private void Execute(Window window)
    {
        
        var selectedItems = ItemsList.Where(item => item.IsChecked);
        
        if (selectedItems.Count() == 0)
        {
            TaskDialog.Show("Ошибка", "Выберите элемент");
        }
        
        HelperParameters helperParameters = new HelperParameters(Context.Document);
        using (Transaction t = new Transaction(Context.Document, "Set Intersectored elements"))
        {
            t.Start();
            foreach (SelectedItems selectedItem in selectedItems)
            {
                List<Element> intersectorItems = FindIntersectingElements(selectedItem.NameItem);
            
                foreach (Element elem in intersectorItems)
                {
                    helperParameters.SetParameter(selectedParameter, elem, value);
                }
            }
            t.Commit();
        }
    }

    private Solid GetSolid(Element element)
    {
        var opt = new Options
        {
            DetailLevel = ViewDetailLevel.Fine
        };
        var geometryElement = element.get_Geometry(opt);

        if (geometryElement == null)
        {
            return null;
        }

        foreach (var geomObj in geometryElement)
        {
            if (geomObj is Solid solid && solid.Volume > 0)
            {
                return solid;
            }
            else if (geomObj is GeometryInstance geomInstance)
            {
                var instGeom = geomInstance.GetInstanceGeometry();
                if (instGeom == null)
                {
                    continue;
                }

                foreach (var instGeomObj in instGeom)
                {
                    if (instGeomObj is Solid instSolid && instSolid.Volume > 0)
                    {
                        return instSolid;
                    }
                }
            }
        }
        
        return null;
    }
}
