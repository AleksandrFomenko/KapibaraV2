using System.Text;
using Autodesk.Revit.UI;
using Helper.Models;
using Helper.Models.SubComponents;

namespace System_name.Models.CoreModels;

public class CoreSystem
{
    private List<BuiltInCategory> MEP_cats = new List<BuiltInCategory>()
    {
        BuiltInCategory.OST_DuctCurves,
        BuiltInCategory.OST_FlexDuctCurves,
        BuiltInCategory.OST_DuctInsulations,
        BuiltInCategory.OST_DuctLinings,
        BuiltInCategory.OST_MechanicalEquipment,
        BuiltInCategory.OST_DuctAccessory,
        BuiltInCategory.OST_DuctTerminal,
        BuiltInCategory.OST_DuctFitting,
        BuiltInCategory.OST_PipeCurves,
        BuiltInCategory.OST_PipeInsulations,
        BuiltInCategory.OST_PipeAccessory,
        BuiltInCategory.OST_PipeFitting,
        BuiltInCategory.OST_MechanicalEquipment,
        BuiltInCategory.OST_Sprinklers,
        BuiltInCategory.OST_PlumbingFixtures,
        BuiltInCategory.OST_FlexPipeCurves
    };
    

    // залупа 
    private string getSystemType(Element elem)
    {
        var result = "";
        if (elem is not FamilyInstance fi) return result;
        var nameList = new List<string>();
        var mp = fi.MEPModel;
        if (mp?.ConnectorManager?.Connectors == null) return result;
        foreach (Connector connector in mp.ConnectorManager.Connectors)
        {
            if (connector.MEPSystem is not MEPSystem ms) continue;
            var msType = Context.Document.GetElement(ms.GetTypeId());
            if (msType != null)
            {
                nameList.Add(msType.get_Parameter(BuiltInParameter.RBS_SYSTEM_ABBREVIATION_PARAM).AsString());
            }
        }

        if (nameList.Count > 0)
        {
            var sb = new StringBuilder();
            foreach (var StringInfo in nameList)
            {
                sb.Append(StringInfo);
                sb.Append(",");
            }

            sb.Length--;
            result = sb.ToString();
        }

        nameList.Clear();
        return result;
    }

    public void Execute(List<Element> elements, string parametersUser, bool systemName)
    {
        var bp = systemName
            ? BuiltInParameter.RBS_SYSTEM_NAME_PARAM
            : BuiltInParameter.RBS_DUCT_PIPE_SYSTEM_ABBREVIATION_PARAM;
        
        var hp = new HelperParameters(Context.Document);
        foreach (var elem in elements)
        {
            var parameter = elem.get_Parameter(bp);
            if (parameter == null)
            {
                return;
            }

            if (parameter.AsString() != null && parameter.AsString() != "")
            {
                if (elem is FamilyInstance notSuperComponent && notSuperComponent.SuperComponent == null)
                {
                    hp.SetParameter(parametersUser, notSuperComponent, parameter.AsString());

                    foreach (var sub in SubComponents.GetSubComponents(notSuperComponent))
                    {
                        hp.SetParameter(parametersUser, sub, parameter.AsString());
                    }
                }

                if (elem is not FamilyInstance)
                {
                    hp.SetParameter(parametersUser, elem, parameter.AsString());
                }
            }
            else
            {
                if (elem is FamilyInstance notSuperComponent && notSuperComponent.SuperComponent == null)
                {
                    var result = getSystemType(notSuperComponent);
                    hp.SetParameter(parametersUser, notSuperComponent, result);

                    foreach (var subelem in SubComponents.GetSubComponents(notSuperComponent))
                    {
                        hp.SetParameter(parametersUser, subelem, result);
                        foreach (Element subelemSecond in SubComponents.GetSubComponents(subelem))
                        {
                            hp.SetParameter(parametersUser, subelemSecond, result);
                        }
                    }
                }
            }
        }
    }
}