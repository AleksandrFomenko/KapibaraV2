using Autodesk.Revit.ApplicationServices;

namespace FamilyCleaner.Models.Open;

public class FamilyOpener
{
    private Application _app;
    

    public static Document OpenFamily(string path)
    {
        var doc = Context.Application.OpenDocumentFile(path);
        return doc;
    }

}