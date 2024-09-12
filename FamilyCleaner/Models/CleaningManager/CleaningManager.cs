namespace FamilyCleaner.Models.FamilyCleaning;

public class CleaningManager
{
    private static List<ElementId> GetPurgeableElements(Document doc, List<PerformanceAdviserRuleId> performanceAdviserRuleIds)
    {
        var failureMessages = PerformanceAdviser.GetPerformanceAdviser().ExecuteRules(doc, performanceAdviserRuleIds).ToList();
        if (failureMessages.Count <= 0) return null;
        var purgeableElementIds = failureMessages[0].GetFailingElements().ToList();
        return purgeableElementIds;
    }
    
    private static void Purge(Document doc)
    {
        const string PurgeGuid = "e8c63650-70b7-435a-9010-ec97660c1bda";

        var performanceAdviserRuleIds = new List<PerformanceAdviserRuleId>();
        
        foreach(var performanceAdviserRuleId in PerformanceAdviser.GetPerformanceAdviser().GetAllRuleIds())
        {
            if (performanceAdviserRuleId.Guid.ToString() != PurgeGuid) continue;
            performanceAdviserRuleIds.Add(performanceAdviserRuleId);
            break;
        }
        
        var purgeableElementIds = GetPurgeableElements(doc, performanceAdviserRuleIds);
        try
        {
            if (purgeableElementIds != null)
            {
                doc.Delete(purgeableElementIds);
            }
        }
        catch (Exception e)
        {
        }

    }

    private static void delete(Document doc)
    {
        var lines = new FilteredElementCollector(doc)
            .OfClass(typeof(LinePatternElement))
            .Select(x => x.Id)
            .ToList();

        doc.Delete(lines);
   
            var fillPatterns = new FilteredElementCollector(doc)
            .OfClass(typeof(FillPatternElement))
            .Where(x => x.Name.ToString() != "Сплошная заливка")
            .Select(x => x.Id)
            .ToList();
        
        doc.Delete(fillPatterns);
    }
    
    public static void CleaningFamily(Document doc)
    {
        delete(doc);
        Purge(doc);
    }
}