using Autodesk.Revit.UI;

namespace KapibaraCore.Parameters;

public class Parameters
{
    public static List<Parameter> GetParameters(Element elem)
    {
        return elem.Parameters.Cast<Parameter>().ToList();
    }

    public static List<FamilyParameter> GetParameterFromFamily(Document doc, Element elem)
    {
        var famParameters = new List<FamilyParameter>();
        Family family = elem switch
        {
            FamilySymbol symbol => symbol.Family,
            
            FamilyInstance instance => (doc.GetElement(instance.GetTypeId()) as FamilySymbol)?.Family,
            
            _ => null
        };
        
        if (family != null)
        {
            Document familyDoc = doc.EditFamily(family);
            famParameters = familyDoc.FamilyManager.GetParameters().ToList();
        }
        return famParameters;
    }
    
    public static List<string> GetParameterFromFamilyAsString(Document doc, Element elem)
    {
        var famParameters = new List<string>();
        Family family = elem switch
        {
            FamilySymbol symbol => symbol.Family,
            
            FamilyInstance instance => (doc.GetElement(instance.GetTypeId()) as FamilySymbol)?.Family,
            
            _ => null
        };
        
        if (family != null)
        {
            Document familyDoc = doc.EditFamily(family);
            famParameters = familyDoc.FamilyManager.GetParameters().Select(x => x.Definition.Name).ToList();
        }
        return famParameters;
    }
}