using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Events;

namespace KapibaraV2.Models.BIM.ExportModels.OpenDoc
{
    public class FailureProcessorOpenDocument
    {
        private IList<FailureDefinitionId> dimensionFailures;

        public FailureProcessorOpenDocument()
        {
            dimensionFailures = GetDimensionFailureIds();
        }

        public void HandleFailures(object sender, FailuresProcessingEventArgs e)
        {
            FailuresAccessor failuresAccessor = e.GetFailuresAccessor();
            IList<FailureMessageAccessor> failureMessages = failuresAccessor.GetFailureMessages();

            foreach (FailureMessageAccessor fma in failureMessages)
            {
                FailureDefinitionId id = fma.GetFailureDefinitionId();

                if (dimensionFailures.Contains(id))
                {
                    failuresAccessor.DeleteWarning(fma);
                }
            }

            e.SetProcessingResult(FailureProcessingResult.Continue);
        }

        private IList<FailureDefinitionId> GetDimensionFailureIds()
        {
            Type dimensionFailuresType = typeof(BuiltInFailures.DimensionFailures);
            PropertyInfo[] properties = dimensionFailuresType.GetProperties(BindingFlags.Public | BindingFlags.Static);
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
