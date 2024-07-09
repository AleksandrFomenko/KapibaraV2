using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Events;

namespace KapibaraV2.Models.BIM.ExportModels.OpenDoc
{
    public class FailureProcessorOpenDocument
    {
        private IList<FailureDefinitionId> relevantFailures;

        public FailureProcessorOpenDocument()
        {
            relevantFailures = GetRelevantFailureIds();
        }

        public void HandleFailures(object sender, FailuresProcessingEventArgs e)
        {
            FailuresAccessor failuresAccessor = e.GetFailuresAccessor();
            IList<FailureMessageAccessor> failureMessages = failuresAccessor.GetFailureMessages();

            foreach (FailureMessageAccessor fma in failureMessages)
            {
                FailureDefinitionId id = fma.GetFailureDefinitionId();

                if (relevantFailures.Contains(id))
                {
                    failuresAccessor.DeleteWarning(fma);
                }
            }

            e.SetProcessingResult(FailureProcessingResult.Continue);
        }

        private IList<FailureDefinitionId> GetRelevantFailureIds()
        {
            List<FailureDefinitionId> failureIds = new List<FailureDefinitionId>();

            failureIds.AddRange(GetFailureIdsFromType(typeof(BuiltInFailures.DimensionFailures)));
            failureIds.AddRange(GetFailureIdsFromType(typeof(BuiltInFailures.CopyMonitorFailures)));
            failureIds.AddRange(GetFailureIdsFromType(typeof(BuiltInFailures.LinkFailures)));

            return failureIds;
        }

        private IList<FailureDefinitionId> GetFailureIdsFromType(Type failureType)
        {
            PropertyInfo[] properties = failureType.GetProperties(BindingFlags.Public | BindingFlags.Static);
            List<FailureDefinitionId> failureIds = new List<FailureDefinitionId>();

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType == typeof(FailureDefinitionId))
                {
                    FailureDefinitionId failureId = (FailureDefinitionId)property.GetValue(null);
                    failureIds.Add(failureId);
                }
            }

            return failureIds;
        }
    }
}
