﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;



namespace KapibaraV2.Helper
{
    internal class Helper
    {
        public Helper()
        {

        }


        public List<Element> GetSubComponents(Element element)
        {
            List<Element> subComponents = new List<Element>();

            if (element is FamilyInstance familyInstance)
            {
                var subComponentIds = familyInstance.GetSubComponentIds();
                subComponents = subComponentIds.Select(id => element.Document.GetElement(id)).ToList();
            }

            return subComponents;
        }
        public void setParameterValueByNameToElement(Element elem, string parameterName, string value)
        {
            Parameter par = elem.LookupParameter(parameterName);

            if (par != null && !par.IsReadOnly)
            {
                par.Set(value);
            }
        }
    }
}
