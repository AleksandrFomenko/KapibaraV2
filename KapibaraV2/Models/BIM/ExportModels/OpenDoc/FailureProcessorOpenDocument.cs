using Autodesk.Revit.UI.Events;
using Autodesk.Revit.DB.Events;

namespace KapibaraV2.Models.BIM.ExportModels.OpenDoc
{
    public class FailureProcessorOpenDocument
    {
        
        public void ApplicationOnFailuresProcessing(object sender, FailuresProcessingEventArgs e)
        {
            FailuresAccessor accessor = e.GetFailuresAccessor();
            accessor.DeleteAllWarnings();
            
            accessor.ResolveFailures(accessor.GetFailureMessages());
            
            ElementId[] elementIds = accessor.GetFailureMessages()
                .SelectMany(item => item.GetFailingElementIds())
                .ToArray();
            
            if (elementIds.Length > 0)
            {
                accessor.DeleteElements(elementIds);
                e.SetProcessingResult(FailureProcessingResult.ProceedWithCommit); 
            }
            else
            {
                e.SetProcessingResult(FailureProcessingResult.Continue); 
            }
        }
        public void UIApplicationOnDialogBoxShowing(object sender, DialogBoxShowingEventArgs e)
        {
            e.OverrideResult(1);
        }
    }
}