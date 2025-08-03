using KapibaraCore.Parameters;

namespace ViewByParameter.Models;

public class ViewByParameterModel(Document document) : IViewByParameterModel
{
    public List<ViewOption?> GetViewOption()
    {
        return
        [
            GetViewOption("3D вид", ViewFamily.ThreeDimensional),
            GetViewOption("План", ViewFamily.FloorPlan),
        ];
    }

    private ViewOption GetViewOption(string name, ViewFamily viewFamily)
    {
        var types = new FilteredElementCollector(document)
            .OfClass(typeof(ViewFamilyType))
            .Cast<ViewFamilyType>()
            .Where(v => v.ViewFamily == viewFamily)
            .Select(v => new Type(v.Name))
            .ToList();

        return new ViewOption(name, types);
    }
    

    public List<FilterOption?> GetFilterOption()
    {
        return
        [
            new FilterOption("Не равно", "CreateNotEqualsRule"),
            new FilterOption("Равно", "CreateEqualsRule"),
            new FilterOption("Не содержит", "CreateNotContainsRule"),
            new FilterOption("Содержит", "CreateContainsRule"),
            new FilterOption("Начинается с", "CreateBeginsWithRule"),
            new FilterOption("Не начинается с", "CreateNotBeginsWithRule"),
            new FilterOption("Заканчивается на", "CreateEndsWithRule"),
            new FilterOption("Не заканчивается на", "CreateNotEndsWithRule")
        ];
    }

    public List<FilterFromProject?> GetFiltersFromProject()
    {
        return [..new List<FilterFromProject?>(new List<FilterFromProject?>())];
    }

    public List<string?> GetProjectParameters()
    {
        return document.GetProjectParameters();
    }

    public List<ElementsByParameter> GetElementsByParameter()
    {
        return
        [
            new ElementsByParameter("а", 1)
        ];
    }
    
    

    public void Execute()
    {
       
    }
}