using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

namespace ViewManager.Sheets.Tabs.CreateSheets.Model;

internal class FailureProcessorNumerator
{
    public void ApplicationOnFailuresProcessing(object sender, FailuresProcessingEventArgs e)
    {
        FailuresAccessor accessor = e.GetFailuresAccessor();
        IList<FailureMessageAccessor> failureMessages = accessor.GetFailureMessages();
    
        var sheetFailureDefIds = new[]
        {
            BuiltInFailures.SheetFailures.SheetNumberDuplicated
        };

        bool handled = false;
        foreach (var message in failureMessages)
        {
            FailureDefinitionId failId = message.GetFailureDefinitionId();
            if (sheetFailureDefIds.Contains(failId))
            {
                accessor.DeleteWarning(message);
                handled = true;
            }
        }
        
        if (handled)
        {
            e.SetProcessingResult(FailureProcessingResult.ProceedWithCommit);
        }
        else
        {
            e.SetProcessingResult(FailureProcessingResult.Continue);
        }
    }
}