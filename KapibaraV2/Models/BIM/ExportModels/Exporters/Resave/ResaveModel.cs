using KapibaraV2.Core;
using Autodesk.Revit.UI;
using System.IO;

namespace KapibaraV2.Models.BIM.ExportModels.Exporters.Resave
{
    public class ResaveModel : IExporter
    {
        private List<String> _paths;
        private string _directoryPath;
        private string _badNameWorkset;
        public ResaveModel(List<string> paths, string directoryPath, string badNameWorkset)
        {
            _paths = paths;
            _directoryPath = directoryPath;
            _badNameWorkset = badNameWorkset;
        }


        private void resaving(string modelPathStr, string destFilepath)
        {
            Autodesk.Revit.ApplicationServices.Application app = RevitApi.UiApplication.Application;
            ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(modelPathStr);
            try
            {
                app.CopyModel(modelPath, destFilepath, true);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("ой-ой-ой", "Какая-то ебучая ошибка с пересохранением, ну в общем вот она:" + ex.Message);

            }
        }
        public string GetModelNameFromPath(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }

        public void Export()
        {
            foreach (string mp in this._paths)
            {
                string modelName = GetModelNameFromPath(mp);
                string destFilePath = Path.Combine(this._directoryPath, modelName + ".rvt");
                resaving(mp, destFilePath);
                UpdateTransmissionData(destFilePath);
            }
        }
        private void UpdateTransmissionData(string modelFilePath)
        {
            ModelPath localModelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(modelFilePath);
            TransmissionData transData = TransmissionData.ReadTransmissionData(localModelPath);

            if (transData == null)
            {
                return;
            }
            
            ICollection<ElementId> externalReferences = transData.GetAllExternalFileReferenceIds();
            foreach (ElementId refId in externalReferences)
            {
                ExternalFileReference extRef = transData.GetLastSavedReferenceData(refId);
                if (extRef.ExternalFileReferenceType == ExternalFileReferenceType.RevitLink)
                {
                    transData.SetDesiredReferenceData(refId, extRef.GetPath(), extRef.PathType, false);
                }
            }

            transData.IsTransmitted = true;
            TransmissionData.WriteTransmissionData(localModelPath, transData);
        }
    }
}
