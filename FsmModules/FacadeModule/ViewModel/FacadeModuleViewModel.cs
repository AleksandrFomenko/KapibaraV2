using Autodesk.Revit.UI;
using FsmModules.Model;

namespace FsmModules.FacadeModule.ViewModel;

public class FacadeModuleViewModel : IWorker
{
    private Document _doc;

    internal FacadeModuleViewModel()
    {
        
    }
    public void Start()
    {
        TaskDialog.Show("ЫА", "Фасад");
    }
}