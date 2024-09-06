namespace Helper.Models.SubComponents;

public static class SubComponents
{
    public static List<Element> GetSubComponents(Element element)
    {
        List<Element> subComponents = new List<Element>();

        if (element is FamilyInstance familyInstance)
        {
            var subComponentIds = familyInstance.GetSubComponentIds();
            subComponents = subComponentIds.Select(id => element.Document.GetElement(id)).ToList();
        }

        return subComponents;
    }
    
}