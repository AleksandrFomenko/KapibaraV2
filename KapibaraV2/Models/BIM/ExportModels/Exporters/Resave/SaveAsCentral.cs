using KapibaraV2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using KapibaraV2.Models.BIM.ExportModels.OpenDoc;
using Autodesk.Revit.DB;

namespace KapibaraV2.Models.BIM.ExportModels.Exporters.Resave
{
    public class SaveAsCentral : IExporter
    {
        private List<String> _paths;
        private string _directoryPath;
        private string _badNameWorkset;
        public SaveAsCentral(List<string> paths, string directoryPath, string badNameWorkset)
        {
            _paths = paths;
            _directoryPath = directoryPath;
            _badNameWorkset = badNameWorkset;
        }

        public void Export()
        {
            OpenDocument openDocument = new OpenDocument();
            foreach (string path in _paths)
            {
                Document doc = openDocument.OpenDocumentAsDetach(path, _badNameWorkset, true, true);

                if (doc == null)
                { 
                    return;
                }

                if (doc.IsLinked)
                {
                    doc.Close(false);
                    return;
                }
                SaveAsOptions saveAsOptions = new SaveAsOptions();
                saveAsOptions.OverwriteExistingFile = true;
                
                ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(path);

                if (doc.IsWorkshared)
                {
                    WorksharingSaveAsOptions worksharingSaveAsOptions = new WorksharingSaveAsOptions();
                    worksharingSaveAsOptions.SaveAsCentral = true;
                    saveAsOptions.SetWorksharingOptions(worksharingSaveAsOptions);
                    
                }

                string modelName = GetModelNameFromPath(path);
                string destFilePath = Path.Combine(_directoryPath, modelName + ".rvt");
                ModelPath modelSavePath = ModelPathUtils.ConvertUserVisiblePathToModelPath(destFilePath);

                doc.SaveAs(modelSavePath, saveAsOptions);
                doc.Close(false);
            }
        }

        public string GetModelNameFromPath(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }
    }
}