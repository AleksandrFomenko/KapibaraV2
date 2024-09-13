using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FamilyCleaner.Models.FamilyCleaning
{
    public class CleaningManager
    {
        public static int modifiedByDeleteMaterial = 0;
        public static bool checkForPurgeMaterials = false;
        public static Document _doc = null;
        public static string materialName = "";
        public int TryCount { get; set; } = 5;
        public bool WithThermals { get; set; } = true;
        public bool WithMaterials { get; set; } = true;
        public bool WithStructures { get; set; } = true;
        public bool WithAppearances { get; set; } = true;
        public bool WithSymbols { get; set; } = true;
        public bool WithLinkSymbols { get; set; } = true;
        public bool WithFamilies { get; set; } = true;
        public bool WithNonDeletable { get; set; } = true;
        public bool WithImportCategories { get; set; } = true;

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

            foreach (var performanceAdviserRuleId in PerformanceAdviser.GetPerformanceAdviser().GetAllRuleIds())
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

        private static void DeleteUnnecessaryElements(Document doc)
        {
            var lines = new FilteredElementCollector(doc)
                .OfClass(typeof(LinePatternElement))
                .Select(x => x.Id)
                .ToList();

            doc.Delete(lines);

            var fillPatterns = new FilteredElementCollector(doc)
                .OfClass(typeof(FillPatternElement))
                .Where(x => !x.Name.ToString().Contains("Сплошная заливка"))
                .Select(x => x.Id)
                .ToList();

            doc.Delete(fillPatterns);
        }

        public static void CleaningFamily(Document doc)
        {
            DeleteUnnecessaryElements(doc);
            Purge(doc);
        }
        
        public void test(Document doc)
        {
            var document = doc;
            for(var i = 1; i <= TryCount; i++) {
                
                using(var transaction = new Transaction(document)) {
                    transaction.Start($"BIM: Remove unused [{i}].");

                    foreach(ElementId elementId in GetElementIds(document)) {
                        try {
                            document.Delete(elementId);
                        } catch {
                           
                        }
                    }

                    transaction.Commit();
                }
            }
        }
        
        private IEnumerable<ElementId> GetElementIds(Document document) {
            return GetMethods(document)
                .SelectMany(item => (ICollection<ElementId>) item.Invoke(document, Array.Empty<object>()))
                .Distinct();
        }
        
        private IEnumerable<MethodInfo> GetMethods(Document document) {
            if(WithThermals) {
                yield return document.GetType().GetMethod("GetUnusedThermals",
                    BindingFlags.Instance | BindingFlags.NonPublic);
            }

            if(WithStructures) {
                yield return document.GetType().GetMethod("GetUnusedStructures",
                    BindingFlags.Instance | BindingFlags.NonPublic);
            }

            if(WithAppearances) {
                yield return document.GetType().GetMethod("GetUnusedAppearances",
                    BindingFlags.Instance | BindingFlags.NonPublic);
            }

            if(WithMaterials) {
                yield return document.GetType().GetMethod("GetUnusedMaterials",
                    BindingFlags.Instance | BindingFlags.NonPublic);
            }

            if(WithImportCategories) {
                yield return document.GetType().GetMethod("GetUnusedImportCategories",
                    BindingFlags.Instance | BindingFlags.NonPublic);
            }

            if(WithNonDeletable) {
                yield return document.GetType().GetMethod("GetNonDeletableUnusedElements",
                    BindingFlags.Instance | BindingFlags.NonPublic);
            }

            if(WithLinkSymbols) {
                yield return document.GetType().GetMethod("GetUnusedLinkSymbols",
                    BindingFlags.Instance | BindingFlags.NonPublic);
            }

            if(WithSymbols) {
                yield return document.GetType().GetMethod("GetUnusedSymbols",
                    BindingFlags.Instance | BindingFlags.NonPublic);
            }

            if(WithFamilies) {
                yield return document.GetType().GetMethod("GetUnusedFamilies",
                    BindingFlags.Instance | BindingFlags.NonPublic);
            }
        }
        
    }
}
