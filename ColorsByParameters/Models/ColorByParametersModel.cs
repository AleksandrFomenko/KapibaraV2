using Autodesk.Revit.DB;

namespace ColorsByParameters.Models;

internal class ColorByParametersModel
{
    private Document _doc;

    internal ColorByParametersModel(Document doc)
    {
        _doc = doc;
    }

    internal string getParameters()
    {
        return "а";
    }
    
}