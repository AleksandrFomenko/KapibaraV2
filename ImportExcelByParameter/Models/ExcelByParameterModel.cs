using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using ImportExcelByParameter.Models.excel;
using ImportExcelByParameter.ViewModels;
using ImportExcelByParameter.Views;

namespace ImportExcelByParameter.Models;

internal class ExcelByParameterModel
{
    private Document _doc;
    internal readonly Data Data;
    internal ExcelWorker Excel;

    internal ExcelByParameterModel(Document doc)
    {
        Data = new Data(doc);
        Excel = new ExcelWorker();
        _doc = doc;
    }

    internal void SetParameterName(string parameterName)
    {
        Excel.ParameterName = parameterName;
    }

    internal void SetSheetName(string sheetName)
    {
        Excel.SheetName = sheetName;
    }
    internal void Execute(string path, string cat)
    {
        Excel.OpenExcel(path);
        Dictionary<string, List<Element>> elementsDict = GetElements(cat);
        
        if (elementsDict == null || elementsDict.Count == 0) return;
        using (var t = new Transaction(_doc, "Import excel by parameter"))
        {
            t.Start();

            foreach (var key in elementsDict.Keys)
            {
                Dictionary<string, string> resultDict = Excel.Execute(key);
                foreach (var kvp in resultDict)
                {
                    string paremeterName = kvp.Key;
                    string parameterValue = kvp.Value;
                    foreach (var elem in elementsDict[key])
                    {
                        var par = KapibaraCore.Parameters.Parameters.GetParameterByName(_doc, elem, paremeterName);
                        if (par != null && par.StorageType == StorageType.ElementId) continue;
                        KapibaraCore.Parameters.Parameters.SetParameterValue(par, parameterValue);
                    }
                }
            }

            TaskDialog.Show("1", "всё");
            
            t.Commit();
        }
        ImportExcelByParameterViewModel.CloseWindow?.Invoke();
    }

    private Dictionary<string, List<Element>> GetElements(string cat)
    {
        var category = _doc.Settings.Categories
            .Cast<Category>()
            .FirstOrDefault(c => c.Name.Equals(cat, StringComparison.OrdinalIgnoreCase));
        if (category == null)
        {
            return null;
        }
        var builtInCat = Data.GetBuiltInCategory(category);
        if (builtInCat == BuiltInCategory.INVALID)
        {
            return null;
        }
        Dictionary<string, List<Element>> elementsDictionary = new Dictionary<string, List<Element>>(StringComparer.OrdinalIgnoreCase);
        var elems = new FilteredElementCollector(_doc)
            .OfCategory(builtInCat)
            .WhereElementIsNotElementType()
            .ToElements();
        foreach (var elem in elems)
        {
            var par = KapibaraCore.Parameters.Parameters.GetParameterByName(_doc, elem, Excel.ParameterName);
            if (par == null) continue;

            string paramValue = par.AsString() ?? par.AsValueString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(paramValue))
                continue;

            if (!elementsDictionary.TryGetValue(paramValue, out var list))
            {
                list = new List<Element>();
                elementsDictionary[paramValue] = list;
            }
            list.Add(elem);
        }
        return elementsDictionary;
    }
}