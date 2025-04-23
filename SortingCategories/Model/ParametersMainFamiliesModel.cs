using System.Collections.ObjectModel;
using System.Diagnostics;
using KapibaraCore.Parameters;
using SortingCategories.ViewModels;

namespace SortingCategories.Model;

public class ParametersMainFamiliesModel
{
    private readonly Document _doc;
    public ObservableCollection<RevitCategory> RevitCategories;
    public ParametersMainFamiliesModel(Document doc)
    {
        _doc = doc;
    }
    public  List<Option> GetOptions()
    {
        return
        [
            new Option("Все элементы", false),
            new Option("Только на активном виде", true)
        ];
    }

    public List<Category> GetCategory()
    {
        var categories = _doc.Settings.Categories;
        return categories
            .Cast<Category>()
            .Where(i => i.IsVisibleInUI)
            .Where(i => i.AllowsBoundParameters)
            .Where(i => i.CategoryType == CategoryType.Model)
            .ToList();
    }

    public List<string> GetParameters()
    {
        return _doc.GetProjectParameters();
    }

    public ObservableCollection<RevitCategory> GetPattern(int i, List<Category> projectCategory)
    {
       return Pattern.GenerateRevitCategories(_doc, projectCategory, i);
    }

    public void Execute(string parameterForSort, string parameterForGroup, bool isActiveView, bool checkSubComponents)
    {
        using (var t = new Transaction(_doc, "Sorting"))
        {
            t.Start();
            
            var revitCategories = RevitCategories.Where(cat => cat.IsChecked).ToList();
            foreach (var revitCat in revitCategories)
            {
                var fec = isActiveView
                    ? new FilteredElementCollector(_doc, _doc.ActiveView.Id)
                    : new FilteredElementCollector(_doc);
                var elems = fec
                    .OfCategory((BuiltInCategory)revitCat.Category.Id.IntegerValue)
                   .WhereElementIsNotElementType()
                   .ToElements();
                
                foreach (var elem in elems)
                {
                    if (elem is FamilyInstance familyInstance && !checkSubComponents)
                    {
                        if (familyInstance.SuperComponent != null) continue;
                    }
                    var parameterSort = elem.GetParameterByName(parameterForSort);
                    parameterSort.SetParameterValue(revitCat.Sorting);
                    var parameterGroup = elem.GetParameterByName(parameterForGroup);
                    parameterGroup.SetParameterValue(revitCat.Group);
                }
            }

            t.Commit();
        }
    }
}