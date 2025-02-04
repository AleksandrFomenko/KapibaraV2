namespace KapibaraV2.Models.BIM.ExportModels
{
    public partial class ModelPathForList : ObservableObject
    {
        [ObservableProperty]
        private string path;

        [ObservableProperty]
        private bool isChecked;
    }
}