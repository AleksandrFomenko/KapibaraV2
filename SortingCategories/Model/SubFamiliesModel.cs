using System.Collections.ObjectModel;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Events;
using KapibaraCore.Elements;
using KapibaraCore.Parameters;
using SortingCategories.ViewModels;

namespace SortingCategories.Model;

public class SubFamiliesModel(Document document)
{
    public List<string> GetParameters()
    {
        return document.GetProjectParameters();
    }

    public ObservableCollection<Algorithm> GetAlgorithms ()
    {
        return new ObservableCollection<Algorithm>()
        {
            new Algorithm("Значение родительского + 1", IncreaseByOne),
            new Algorithm("Значение родительского + 0.1", IncreaseByOneTenth),
            new Algorithm("Значение родительского + 1 * n", IncreaseByOneMultiply),
            new Algorithm("Значение родительского + 0.1 * n", IncreaseByOneTenthMultiply),
            new Algorithm("Значение родительского", Equate)
        };
    }

    private List<Element> GetElements()
    {
        return new FilteredElementCollector(document,document.ActiveView.Id)
            .WhereElementIsNotElementType()
            .ToElements()
            .ToList();
    }
    

    private void IncreaseByOne(string parameterSort, string parameterGroup, string groupValue)
    {
        
        var elements = GetElements();
        using (var t = new Transaction(document, "Sorting"))
        {
            t.Start();
            foreach (var element in elements)
            {
                var valueStr = element.GetParameterByName(parameterSort)?.AsValueString();

                if (!double.TryParse(valueStr, System.Globalization.NumberStyles.Any, 
                        System.Globalization.CultureInfo.InvariantCulture, out var value))
                    value = 0; 

                foreach (var subelement in element.GetAllSubComponents())
                {
                    var param = subelement.GetParameterByName(parameterSort);
                    param.SetParameterValue(value + 1);
                    subelement.GetParameterByName(parameterGroup)?.SetParameterValue(groupValue);
                }
            }
            t.Commit();
        }
    }
    
    private void IncreaseByOneTenth(string parameterSort, string parameterGroup, string groupValue)
    {
        
        var elements = GetElements();
        using (var t = new Transaction(document, "Sorting"))
        {
            t.Start();
            foreach (var element in elements)
            {
                var valueStr = element.GetParameterByName(parameterSort)?.AsValueString();

                if (!double.TryParse(valueStr, System.Globalization.NumberStyles.Any, 
                        System.Globalization.CultureInfo.InvariantCulture, out var value))
                    value = 0; 

                foreach (var subelement in element.GetAllSubComponents())
                {
                    var param = subelement.GetParameterByName(parameterSort);
                    if (param != null && param.StorageType == StorageType.Double)
                    {
                        param.SetParameterValue(value + 0.1);
                    }

                    subelement.GetParameterByName(parameterGroup)?.SetParameterValue(groupValue);
                }
            }
            t.Commit();
        }
    }
    
    private void IncreaseByOneMultiply(string parameterSort, string parameterGroup, string groupValue)
    {
        
        var elements = GetElements();
        using (var t = new Transaction(document, "Sorting"))
        {
            t.Start();
            foreach (var element in elements)
            {
                var valueStr = element.GetParameterByName(parameterSort)?.AsValueString();

                if (!double.TryParse(valueStr, System.Globalization.NumberStyles.Any, 
                        System.Globalization.CultureInfo.InvariantCulture, out var value))
                    value = 0;

                var count = 1;
                foreach (var subelement in element.GetAllSubComponents())
                {
                    var param = subelement.GetParameterByName(parameterSort);
                    if (param != null && param.StorageType == StorageType.Double)
                    {
                        param.SetParameterValue(value + count);
                    }

                    count++;

                    subelement.GetParameterByName(parameterGroup)?.SetParameterValue(groupValue);
                }
            }
            t.Commit();
        }
    }
    
    private void IncreaseByOneTenthMultiply(string parameterSort, string parameterGroup, string groupValue)
    {
        
        var elements = GetElements();
        using (var t = new Transaction(document, "Sorting"))
        {
            t.Start();
            foreach (var element in elements)
            {
                var valueStr = element.GetParameterByName(parameterSort)?.AsValueString();

                if (!double.TryParse(valueStr, System.Globalization.NumberStyles.Any, 
                        System.Globalization.CultureInfo.InvariantCulture, out var value))
                    value = 0; 

                var count = 0.1;
                foreach (var subelement in element.GetAllSubComponents())
                {
                    var param = subelement.GetParameterByName(parameterSort);
                    if (param != null && param.StorageType == StorageType.Double)
                    {
                        param.SetParameterValue(value + count);
                    }
                    count += 0.1;
                    subelement.GetParameterByName(parameterGroup)?.SetParameterValue(groupValue);
                }
            }
            t.Commit();
        }
    }

    private void Equate(string parameterSort, string parameterGroup, string groupValue)
    {
        
        var elements = GetElements();
        using (var t = new Transaction(document, "Sorting"))
        {
            t.Start();
            foreach (var element in elements)
            {
                var valueStr = element.GetParameterByName(parameterSort)?.AsValueString();
                foreach (var subelement in element.GetAllSubComponents())
                {
                    var param = subelement.GetParameterByName(parameterSort);
                    if (param != null && param.StorageType == StorageType.Double)
                    {
                        param.SetParameterValue(valueStr);
                    }
                    subelement.GetParameterByName(parameterGroup)?.SetParameterValue(groupValue);
                }
            }
            t.Commit();
        }
    }
}