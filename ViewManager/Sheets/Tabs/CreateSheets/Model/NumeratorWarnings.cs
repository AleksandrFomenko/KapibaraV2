using System.Diagnostics;
using Autodesk.Revit.DB;

namespace ViewManager.Sheets.Tabs.CreateSheets.Model
{
    internal class NumeratorWarnings : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            try
            {
                IList<FailureMessageAccessor> failures = failuresAccessor.GetFailureMessages();

                foreach (var fail in failures)
                {
                    //FailureSeverity severity = fail.GetSeverity();
                    //string description = fail.GetDescriptionText();
                    //Debug.WriteLine($"Тип ошибки: {severity.ToString()}"); 
                    FailureDefinitionId failId = fail.GetFailureDefinitionId();
                    
                    if (failId == BuiltInFailures.SheetFailures.SheetNumberDuplicated)
                    {
                        //Debug.WriteLine($"Описание ошибки: {description}");
                        failuresAccessor.GetFailureHandlingOptions().SetClearAfterRollback(true);
                        return FailureProcessingResult.ProceedWithRollBack;
                    }
                }
            }
            catch (Exception)
            {
                //Debug.WriteLine(ex.ToString());
            }
            
            return FailureProcessingResult.Continue;
        }
    }
}