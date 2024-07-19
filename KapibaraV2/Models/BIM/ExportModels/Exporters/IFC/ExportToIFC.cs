using KapibaraV2.Models.BIM.ExportModels.OpenDoc;
using Autodesk.Revit.UI;
using BIM.IFC.Export.UI;
using System.IO;
using System.Reflection;
using KapibaraV2.Core;
using Newtonsoft.Json;

namespace KapibaraV2.Models.BIM.ExportModels.Exporters.IFC
{
    public class ExportToIfc : IExporter
    {
        private List<String> _paths;
        private string _directoryPath;
        private string _badNameWorkset;
        private string _ifcPath;
        public ExportToIfc(List<string> paths, string directoryPath, string badNameWorkset, string ifcPath)
        {
            _paths = paths;
            _directoryPath = directoryPath;
            _badNameWorkset = badNameWorkset;
            _ifcPath = ifcPath;
        }
        public void Export()
        {
            OpenDocument openDocument = new OpenDocument();
            try
            {
                foreach (string path in this._paths)
                {
                    Document doc = openDocument.OpenDocumentAsDetach(path, this._badNameWorkset, true,
                        false);
                    
                    if (doc != null)
                    {
                        exportToIfc(doc, this._directoryPath, this._ifcPath);
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("не норм", "Что-то пошло не так ()" + ex.Message);
            };
        }

        private void exportToIfc(Document doc, string directoryPath, string jsonConfigPath)
        {
            View3D navisworksViewCollector = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Views)
                .WhereElementIsNotElementType()
                .OfClass(typeof(View3D))
                .Cast<View3D>()
                .FirstOrDefault(v => v.Name.ToLower().Contains("navis"));
            if (navisworksViewCollector == null)
            {
                return;
            }
            IFCExportConfiguration myIFCExportConfiguration = IFCExportConfiguration.CreateDefaultConfiguration();
            using (StreamReader r = new StreamReader(jsonConfigPath))
            {
                string json = r.ReadToEnd();
                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                var converter = new IFCExportConfigurationConverter();
                myIFCExportConfiguration = converter.ConvertFromDictionary(dict);
            }
            try
            {
                if (doc == null)
                {
                    throw new ArgumentNullException(nameof(doc), "Документ null");
                }
                
                using (Transaction t = new Transaction(doc, "ifc export"))
                {
                    t.Start();
                    IFCExportOptions ifcExportOptions = new IFCExportOptions ();
                    myIFCExportConfiguration.ActiveViewId = navisworksViewCollector.Id.IntegerValue;
                    myIFCExportConfiguration.ActivePhaseId = ElementId.InvalidElementId.IntegerValue;;
                    myIFCExportConfiguration.UpdateOptions(ifcExportOptions, navisworksViewCollector.Id);
                    doc.Export(directoryPath, doc.Title, ifcExportOptions);
                    t.Commit();
                }
            }
            catch (Exception ex)
            {
                doc.Close(false);
                TaskDialog.Show("Ошибка", ex.Message + "\n" + ex.StackTrace);
            }
            doc.Close(false);
        }

        private class IFCExportConfigurationConverter
        {
            public IFCExportConfiguration ConvertFromDictionary(Dictionary<string, object> dict)
            {
                var json = JsonConvert.SerializeObject(dict);
                return JsonConvert.DeserializeObject<IFCExportConfiguration>(json);
            }
        }
    }
}
