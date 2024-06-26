using KapibaraV2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System.IO;

namespace KapibaraV2.Models.BIM.ExportModels.ResaveModel
{
    public class ResaveModel
    {
        public ResaveModel() { }


        private void resaving(String modelPathStr, string destFilepath)
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

        public void resavingModels (List<String> modelPaths, String destFolderPath)
        {
            foreach (String mp in modelPaths)
            {
                string modelName = GetModelNameFromPath(mp);
                string destFilePath = Path.Combine(destFolderPath, modelName + ".rvt");

                resaving(mp, destFilePath);
            }

        }
    }
}
