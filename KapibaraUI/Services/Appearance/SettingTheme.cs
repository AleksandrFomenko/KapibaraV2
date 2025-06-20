namespace KapibaraUI.Services.Appearance;

public class ConfigWrapper
{
    public Setting Setting { get; set; }
}
public class Setting(string name, Theme theme)
{
    public string Name { get; } = name;
    public Theme Theme { get; } = theme;
    
}

public enum Theme
{
    Light,
    Dark
}