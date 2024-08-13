namespace Helper.Models;

public class HelperParameters
{
    
    private readonly Document _doc;
    public HelperParameters(Document doc)
    {
        _doc = doc;
    }
    
    private Parameter SearchParameter(string parameterName, Element elem)
    {
        Parameter parameter = elem.LookupParameter(parameterName);
        if (parameter != null)
        {
            return parameter;
        }
        
        var type = _doc.GetElement(elem.GetTypeId());

        if (type != null)
        {
            parameter = type.LookupParameter(parameterName);
            if (parameter != null)
            {
                return parameter;
            }
        }
        return null;
    }

    public void SetParameter<T>(string parameterName, Element elem, T value)
    {
        var parameter = SearchParameter(parameterName, elem);
        if (parameter == null) return;

        if (typeof(T) == typeof(int))
        {
            parameter.Set(Convert.ToInt32(value));
        }
        else if (typeof(T) == typeof(double))
        {
            parameter.Set(Convert.ToDouble(value));
        }
        else if (typeof(T) == typeof(string))
        {
            parameter.Set(value.ToString());
        }
    }
}
