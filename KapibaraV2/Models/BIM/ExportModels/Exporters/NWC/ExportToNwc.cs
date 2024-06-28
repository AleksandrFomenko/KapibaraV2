using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using KapibaraV2.Models.BIM.ExportModels.OpenDoc;

namespace KapibaraV2.Models.BIM.ExportModels.Exporters.NWC
{
    public class ExportToNwc : IExporter
    {
        private List<String> _paths;
        private string _directoryPath;
        private string _badNameWorkset;

        public ExportToNwc (List<string> paths, string directoryPath, string badNameWorkset)
        {
            _paths = paths;
            _directoryPath = directoryPath;
            _badNameWorkset = badNameWorkset;
        }

        public void Export()
        {
            OpenDocument openDocument = new OpenDocument();
            try
            {
                foreach (string path in this._paths)
                {
                    Document doc = openDocument.OpenDocumentAsDetach(path, this._badNameWorkset, true);
                    if (doc != null)
                    {
                        exportToNwc(doc, this._directoryPath);
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("не норм", "Что-то пошло не так ()" + ex.Message);
            };
        }

        private void exportToNwc(Document doc, string directoryPath)
        {
            if (doc == null)
            {
                TaskDialog.Show("Error", "Document is null.");
                return;
            }

            if (!Directory.Exists(directoryPath))
            {
                TaskDialog.Show("Error", "Директория не найдена");
                return;
            }

            View3D navisworksViewCollector = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Views)
                .WhereElementIsNotElementType()
                .OfClass(typeof(View3D))
                .Cast<View3D>()
                .FirstOrDefault(v => v.Name.ToLower().Contains("navisworks"));

            if (navisworksViewCollector == null)
            {
                return;
            }

            var elementsInView = new FilteredElementCollector(doc, navisworksViewCollector.Id)
                .WhereElementIsNotElementType()
                .ToElementIds();

            if (!elementsInView.Any())
            {
                return;
            }

            NavisworksExportOptions options = new NavisworksExportOptions
            {
                ExportElementIds = true,
                Coordinates = NavisworksCoordinates.Shared,
                FacetingFactor = 1,
                ExportUrls = false,
                ConvertLights = false,
                ExportRoomAsAttribute = false,
                ConvertElementProperties = true,
                ConvertLinkedCADFormats = false,
                ExportLinks = false,
                ExportParts = true,
                FindMissingMaterials = true,
                DivideFileIntoLevels = true,
                ExportScope = NavisworksExportScope.View,
                ViewId = navisworksViewCollector.Id,
                ExportRoomGeometry = false
            };

            try
            {
                string docName = doc.Title;
                if (doc.Title.Contains("_detached", StringComparison.OrdinalIgnoreCase))
                {
                    int index = doc.Title.IndexOf("_detached", StringComparison.OrdinalIgnoreCase);
                    docName = doc.Title.Substring(0, index).Trim();
                }

                if (doc.Title.Contains("_отсоединено", StringComparison.OrdinalIgnoreCase))
                {
                    int index = doc.Title.IndexOf("_отсоединено", StringComparison.OrdinalIgnoreCase);
                    docName = doc.Title.Substring(0, index).Trim();
                }

                doc.Export(directoryPath, docName, options);

                bool saveChanges = false;
                doc.Close(saveChanges);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", "Export failed: " + ex.Message);
            }
        }
    }
}
