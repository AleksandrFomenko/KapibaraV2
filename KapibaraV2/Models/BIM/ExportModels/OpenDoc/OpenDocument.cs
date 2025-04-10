﻿
using System.Diagnostics;
using KapibaraV2.Core;


namespace KapibaraV2.Models.BIM.ExportModels.OpenDoc
{
    public class OpenDocument
    {
        public Document OpenDocumentAsDetach(string filePath, string badNameWorkset, bool deleteLinksDwg, 
            bool closeAllWorset)
        {
            var app = RevitApi.UiApplication;
            ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
            Autodesk.Revit.ApplicationServices.Application controlledApp = app.Application;
            
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
                Debug.WriteLine(ex.ToString());

            }
            
            FailureProcessorOpenDocument failureProcessor = null;
            try
            {
                failureProcessor = new FailureProcessorOpenDocument();

                controlledApp.FailuresProcessing += failureProcessor.ApplicationOnFailuresProcessing;
                app.DialogBoxShowing += failureProcessor.UIApplicationOnDialogBoxShowing;
                

                Document openDoc = controlledApp.OpenDocumentFile(modelPath, openOptions);

                if (openDoc != null && openDoc.IsValidObject)
                {
                    if (deleteLinksDwg) { DeleteLinksAndDwg(openDoc); }
                    return openDoc;
                }

                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                throw;
            }
            finally
            {
                if (failureProcessor != null)
                {
                    controlledApp.FailuresProcessing -= failureProcessor.ApplicationOnFailuresProcessing;
                    app.DialogBoxShowing -= failureProcessor.UIApplicationOnDialogBoxShowing;
                }
            }

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
