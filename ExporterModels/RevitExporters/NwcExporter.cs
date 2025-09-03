using System.IO;
using Autodesk.Revit.UI;
using ExporterModels.Progress;

namespace ExporterModels.RevitExporters;

public class NwcExporter : RevitExporter
{
    public async Task ExportSingleAsync(
        UIApplication uiApp,
        string filePath,
        string directoryPath,
        string badNameWorkset,
        IProgress<ProgressInfo>? progress = null)
    {
        try
        {
            Console.WriteLine($"🔧 [NwcExporter] Начинаем экспорт: {filePath}");

            var doc = OpenDocumentAsDetach(filePath, badNameWorkset, true, false);
            if (doc != null)
            {
                ExportToNwc(doc, directoryPath);
                doc.Close(false);
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔧 [NwcExporter] ошибкаа: {ex.Message}");
            throw; 
        }
    }

    private static void ExportToNwc(Document doc, string directoryPath)
    {
        if (doc == null) return;
        if (!Directory.Exists(directoryPath)) return;
        
        var navisworksViewCollector = new FilteredElementCollector(doc)
            .OfCategory(BuiltInCategory.OST_Views)
            .WhereElementIsNotElementType()
            .OfClass(typeof(View3D))
            .Cast<View3D>()
            .FirstOrDefault(v => v.Name.Contains("navisworks", StringComparison.OrdinalIgnoreCase));

        if (navisworksViewCollector == null) return;

        var elementsInView = new FilteredElementCollector(doc, navisworksViewCollector.Id)
            .WhereElementIsNotElementType()
            .ToElementIds();

        if (!elementsInView.Any()) return;

        var options = new NavisworksExportOptions
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
            var docName = PrepareDocumentName(doc.Title);
            doc.Export(directoryPath, docName, options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка экспорта в NWC для {doc.Title}: {ex.Message}");
        }
    }

    private static string PrepareDocumentName(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) return "unknown";

        if (title.Contains("_detached", StringComparison.OrdinalIgnoreCase))
        {
            var index = title.IndexOf("_detached", StringComparison.OrdinalIgnoreCase);
            return title.Substring(0, index).Trim();
        }

        if (title.Contains("_отсоединено", StringComparison.OrdinalIgnoreCase))
        {
            var index = title.IndexOf("_отсоединено", StringComparison.OrdinalIgnoreCase);
            return title.Substring(0, index).Trim();
        }

        return title;
    }
}