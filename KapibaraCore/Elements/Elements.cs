using Autodesk.Revit.DB;

namespace KapibaraCore.Elements;

public static class Elements
{
    public static List<Element> GetAllSubComponents(this Element element)
    {
        var result = new List<Element>();
        if (element is FamilyInstance familyInstance)
        {
            var subComponentIds = familyInstance.GetSubComponentIds();
            foreach (var subId in subComponentIds)
            {
                var subElement = element.Document.GetElement(subId);
                if (subElement != null)
                {
                    result.Add(subElement);
                    result.AddRange(GetAllSubComponents(subElement));
                }
            }
        }
        return result;
    }
}