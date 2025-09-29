using ProjectAxes.Common;
using ProjectAxes.Models;

namespace ProjectAxes.ViewModels;

public sealed partial class ProjectToolViewModel : ToolViewModel
{
    private readonly IModel _model;

    public ProjectToolViewModel(HeaderInfo header, IModel model)
        : base(header)
    {
        _model = model;
    }
    [RelayCommand]
    private void Execute()
    {
        switch (Option?.Type)
        {
            case OptionType.All:
                _model.DoAll(BeginningIsChecked, EndIsChecked);
                break;
            case OptionType.Selection:
                _model.DoSelection(BeginningIsChecked, EndIsChecked);
                break;
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}