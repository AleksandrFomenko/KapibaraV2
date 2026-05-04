using ImportExcelByParameter.Enums;
using ImportExcelByParameter.Models.excel;
using ImportExcelByParameter.ViewModels;
using KapibaraCore.Parameters;


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

    internal void SetParameterName(string parameterName) =>  Excel.ParameterName = parameterName;
    internal void SetSheetName(string sheetName) =>  Excel.SheetName = sheetName;
    internal void SetRowNumber(int rowNumber)  =>  Excel.RowNumber = rowNumber;

    internal void Execute(string path, string cat, SelectionMode selectionMode)
    {
        Excel.OpenExcel(path);
        var elementsDict = GetElements(cat, selectionMode);
        if (elementsDict == null || elementsDict.Count == 0) return;
        using var t = new Transaction(_doc, "Import from excel by parameter");
        t.Start();
        foreach (var key in elementsDict.Keys)
        {
            var resultDict = Excel.Execute(key);
            foreach (var kvp in resultDict)
            {
                foreach (var elem in elementsDict[key])
                {
                    var par = elem.GetParameterByName(kvp.Key);
                    if (par == null || par.StorageType == StorageType.ElementId) continue;
                    Parameters.SetParameterValue(par, kvp.Value);
                }
            }
        }
        t.Commit();
    }
    private Dictionary<string, List<Element>> GetElements(string cat, SelectionMode selectionMode)
    {
        var elems = selectionMode switch
        {
            SelectionMode.ByCategory => GetByCategory(cat),
            SelectionMode.AllOnActiveView => new FilteredElementCollector(_doc, _doc.ActiveView.Id)
                .WhereElementIsNotElementType()
                .ToElements(),
            SelectionMode.AllInProject => new FilteredElementCollector(_doc)
                .WhereElementIsNotElementType()
                .ToElements(),
            _ => []
        };

        var elementsDictionary = new Dictionary<string, List<Element>>(StringComparer.OrdinalIgnoreCase);
        foreach (var elem in elems)
        {
            var par = elem.GetParameterByName(Excel.ParameterName);
            if (par == null) continue;

            var paramValue = par.AsString() ?? par.AsValueString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(paramValue)) continue;

            if (!elementsDictionary.TryGetValue(paramValue, out var list))
            {
                list = [];
                elementsDictionary[paramValue] = list;
            }
            list.Add(elem);
        }
        return elementsDictionary;
    }

    private IEnumerable<Element> GetByCategory(string cat)
    {
        var category = _doc.Settings.Categories
            .Cast<Category>()
            .FirstOrDefault(c => c.Name.Equals(cat, StringComparison.OrdinalIgnoreCase));
        if (category == null) return [];

        var builtInCat = Data.GetBuiltInCategory(category);
        if (builtInCat == BuiltInCategory.INVALID) return [];

        return new FilteredElementCollector(_doc)
            .OfCategory(builtInCat)
            .WhereElementIsNotElementType()
            .ToElements();
    }
}