using System.Windows;

namespace EngineeringSystems.ViewModels;

public class Options
{
    public string NameOpt { get; set; }
    public double Width { get; }
    public double Height { get; }
    public GridLength FirstColumnWidth { get; }
    public GridLength SecondColumnWidth { get; }

    public Options(string name, double width, double height, GridLength firstColumnWidth, GridLength secondColumnWidth)
    {
        NameOpt = name;
        Width = width;
        Height = height;
        FirstColumnWidth = firstColumnWidth;
        SecondColumnWidth = secondColumnWidth;
    }
}

public class SystemParameters
{
    public string Name { get; set; }
    
    public SystemParameters(string name)
    {
        this.Name = name;
    }
}