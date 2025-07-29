namespace ViewByParameter.AddFilter.Models;

public class FilterProject(string name)
{
    public string Name { get; set; } = name;
    public bool IsChecked { get; set; } = false;
}