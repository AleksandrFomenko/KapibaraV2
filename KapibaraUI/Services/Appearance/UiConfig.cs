using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace KapibaraUI.Services.Appearance;

public class UiConfig
{
    public ApplicationTheme Theme { get; set; }
    public WindowBackdropType Backdrop { get; set; }

    public UiConfig()
    {
        Theme = ApplicationTheme.Light;
        Backdrop = WindowBackdropType.Mica;
    }

    public UiConfig(ApplicationTheme theme, WindowBackdropType backdrop)
    {
        Theme = theme;
        Backdrop = backdrop;
    }
}