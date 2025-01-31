namespace ExporterModels.Models.Entities;

public  partial class Project
{

    public string Name { get; set; }
    public string SavePath  { get; set; }
    public List<ModelPath> Models  { get; set; }
    public string BadNameWorkset  { get; set; }
}
public partial class ModelPath : ObservableObject
{
    public bool IsChecked { get; set; }
    private string Path { get; set; }
}