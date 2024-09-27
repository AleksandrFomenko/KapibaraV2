using Autodesk.Revit.UI;
using FsmModules.Model;

namespace FsmModules.WaterSupply.ViewModel;

public class WaterSupplyViewModel : IWorker
{
    private Document _doc;

    internal WaterSupplyViewModel()
    {
    }
    public void Start()
    {
        TaskDialog.Show("ываы", "это вк");
    }
}