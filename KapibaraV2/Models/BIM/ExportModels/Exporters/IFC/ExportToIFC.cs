using KapibaraV2.Models.BIM.ExportModels.OpenDoc;
using Autodesk.Revit.UI;
using BIM.IFC.Export.UI;
using System.IO;
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
                .FirstOrDefault(v => v.Name.ToLower().Contains("navisworks"));

            IFCExportConfiguration myIFCExportConfiguration = IFCExportConfiguration.CreateDefaultConfiguration();

            using (StreamReader r = new StreamReader(jsonConfigPath))
            {
                string json = r.ReadToEnd();
                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                var converter = new IFCExportConfigurationConverter();
                myIFCExportConfiguration = converter.ConvertFromDictionary(dict);
            }

            using (Transaction t = new Transaction(doc, "ifc export"))
            {
                t.Start();
                IFCExportOptions ifcExportOptions = new IFCExportOptions();
                myIFCExportConfiguration.UpdateOptions(ifcExportOptions, navisworksViewCollector.Id);
                doc.Export(directoryPath, doc.Title, ifcExportOptions);
                t.Commit();
                bool saveChanges = false;
                doc.Close(saveChanges);
            }
        }

        private class IFCExportConfigurationConverter
        {
            public IFCExportConfiguration ConvertFromDictionary(Dictionary<string, object> dict)
            {
                // Логика для конвертации словаря в объект IFCExportConfiguration
                var json = JsonConvert.SerializeObject(dict);
                return JsonConvert.DeserializeObject<IFCExportConfiguration>(json);
            }
        }
    }
}
