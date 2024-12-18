using Autodesk.Revit.DB.Events;


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
        
        foreach (var message in failureMessages)
        {
            FailureDefinitionId failId = message.GetFailureDefinitionId();
            if (sheetFailureDefIds.Contains(failId))
            {
                accessor.GetFailureHandlingOptions().SetClearAfterRollback(true);
                accessor.DeleteWarning(message);
                accessor.RollBackPendingTransaction();
                e.SetProcessingResult(FailureProcessingResult.ProceedWithRollBack);
            }
        }
    }
}