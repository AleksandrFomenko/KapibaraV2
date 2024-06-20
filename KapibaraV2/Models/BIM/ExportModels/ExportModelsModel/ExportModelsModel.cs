using Autodesk.Revit.UI;
using KapibaraV2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace KapibaraV2.Models.BIM.ExportModels.ExportModelsModel
{
    class ExportModelsModel
    {
        public ExportModelsModel()
        {

        }

        public Document OpenDocument(string filePath)
        {

            Autodesk.Revit.ApplicationServices.Application app = RevitApi.Document.Application;

            ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);

            OpenOptions openOptions = new OpenOptions();
            Document newDoc = app.OpenDocumentFile(modelPath, openOptions);

            if (newDoc != null && newDoc.IsValidObject)
            {
                return newDoc;
            }

            return null;
        }


        public void exportToNwc(Document doc, string directoryPath)
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
                doc.Export(directoryPath, doc.Title, options);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", "Export failed: " + ex.Message);
            }
        }

        public void ExportModel(string filePath, string directoryPath)
        {

            Document doc = OpenDocument(filePath);
            if (doc == null)
            {
                TaskDialog.Show("Error", "Doc is null!");
                return;
            };
            exportToNwc(doc, directoryPath);

            bool saveChanges = false;
            doc.Close(saveChanges);
        }


        public void Execute(string directoryPath, string[] paths)
        {

            try
            {
                foreach (string path in paths)
                {
                    ExportModel(path, directoryPath);
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("не норм", "Что-то пошло не так ()" + ex.Message);
                return;
            };


        }
    }
}
