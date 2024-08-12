using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using KapibaraV2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapibaraV2.Models.BIM.ExportModels.OpenDoc
{
    public class OpenDocument
    {
        public Document OpenDocumentAsDetach(string filePath, string badNameWorkset, bool deleteLinksDwg, 
            bool closeAllWorset)
        {
            Autodesk.Revit.ApplicationServices.Application app = RevitApi.Document.Application;
            ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
            
            OpenOptions openOptions = new OpenOptions();
            
            try
            {
                openOptions.DetachFromCentralOption = DetachFromCentralOption.DetachAndPreserveWorksets;
                WorksetConfiguration worksetConfiguration = new WorksetConfiguration(WorksetConfigurationOption.CloseAllWorksets);
                if (!closeAllWorset)
                {
                    IList<WorksetPreview> worksets = WorksharingUtils.GetUserWorksetInfo(modelPath);
                    IList<WorksetId> worksetIds;
                    if (string.IsNullOrEmpty(badNameWorkset))
                    {
                        worksetIds = worksets.Select(workset => workset.Id).ToList();
                    }
                    else
                    {
                        worksetIds = worksets
                            .Where(workset => !workset.Name.ToLower().Contains(badNameWorkset.ToLower()))
                            .Select(workset => workset.Id)
                            .ToList();
                    }
                    worksetConfiguration.Open(worksetIds);
                }
                openOptions.SetOpenWorksetsConfiguration(worksetConfiguration);
            }
            catch (Exception ex)
            {

            }


            FailureProcessorOpenDocument failureProcessor = new FailureProcessorOpenDocument();

            app.FailuresProcessing += failureProcessor.ApplicationOnFailuresProcessing;

            Document openDoc = app.OpenDocumentFile(modelPath, openOptions);

            app.FailuresProcessing -= failureProcessor.ApplicationOnFailuresProcessing;


            if (openDoc != null && openDoc.IsValidObject)
            {
                if (deleteLinksDwg) { DeleteLinksAndDwg(openDoc); }
                return openDoc;
            }

            return null;
        }

        private static void DeleteLinksAndDwg(Document doc)
        {

            IList<ElementId> linkedElementIds = new FilteredElementCollector(doc).
                OfCategory(BuiltInCategory.OST_RvtLinks).
                WhereElementIsElementType().
                ToElementIds().
                ToList();

            IList<ElementId> dwgElementIds = new FilteredElementCollector(doc)
                .OfClass(typeof(ImportInstance))
                .ToElements()
                .Select(element => element.Id)
                .ToList();


            using (Transaction tx = new Transaction(doc, "Удаление всех связей и DWG"))
            {
                tx.Start();

                if (linkedElementIds.Count > 0)
                {
                    doc.Delete(linkedElementIds);
                }
                if (dwgElementIds.Count > 0)
                {
                    doc.Delete(dwgElementIds);
                }

                tx.Commit();
            }
        }
    }
}
