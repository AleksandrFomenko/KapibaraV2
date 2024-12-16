namespace KapibaraCore.Parameters;

public class Parameters
{

    public static List<Parameter> GetParameters(Element elem)
    {
        return elem.Parameters.Cast<Parameter>().ToList();
    }
}