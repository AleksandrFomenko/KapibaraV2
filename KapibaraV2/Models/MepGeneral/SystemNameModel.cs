using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using KapibaraV2.Helper;

namespace KapibaraV2.Models.MepGeneral
{
    internal class SystemNameModel
    {
        private List<Element> elems;
        private bool _IsActiveView;
        private string _selectedSystemParameter;
        private BuiltInParameter bp;
        private Helper.Helper _helper;
        private string ParameterName;


        public SystemNameModel(List<Element> elems,bool _isActiveView, string _selectedSystemParameter,string ParameterName) {
            this.elems = elems;
            this._IsActiveView = _isActiveView;
            this._selectedSystemParameter = _selectedSystemParameter;
            this.ParameterName = ParameterName;

            if (_selectedSystemParameter == "Имя системы") { 
                bp = BuiltInParameter.RBS_SYSTEM_NAME_PARAM; 
            }
            else if (_selectedSystemParameter == "Сокращение для системы") {
                bp = BuiltInParameter.RBS_DUCT_PIPE_SYSTEM_ABBREVIATION_PARAM;
            }
            _helper = new Helper.Helper();
        }
        public void Execute()
        {
            foreach (Element elem in elems)
            {
                var par = elem.get_Parameter(bp);
                if (par != null && par.AsString() != null && par.AsString() != "")
                {
                    _helper.setParameterValueByNameToElement(elem, ParameterName, par.AsString());
                    foreach (Element subelem in _helper.GetSubComponents(elem))
                    {
                        _helper.setParameterValueByNameToElement(subelem, ParameterName, par.AsString());
                        foreach (Element subelem_second in _helper.GetSubComponents(subelem))
                        {
                            _helper.setParameterValueByNameToElement(subelem_second, ParameterName, par.AsString());
                        }
                    }
                }
            }
        }
    }
}
