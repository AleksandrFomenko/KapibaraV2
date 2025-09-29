using ProjectAxes.Abstractions;
using ProjectAxes.Common;
using ProjectAxes.Models;
using ProjectAxes.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectAxes.Factories;

public sealed class ViewModelFactory : IViewModelFactory
{
    private readonly IServiceProvider _sp;
    private readonly Dictionary<Type, HeaderInfo> _headers;

    public ViewModelFactory(IServiceProvider sp)
    {
        _sp = sp;
        
        _headers = new Dictionary<Type, HeaderInfo>
        {
            [typeof(MockModel)]        = new HeaderInfo("Заголовок",      "Выберите что-то"),
            [typeof(ProjectAxesModel)] = new HeaderInfo("Project Axes",   "Выберите оси"),
            [typeof(ProjectLevelsModel)] = new HeaderInfo("Project Levels","Выберите уровни"),
        };
    }

    public IViewModel Create<TModel>(TModel model) where TModel : class, IModel
    {
        var modelType = typeof(TModel);

        if (!_headers.TryGetValue(modelType, out var header))
            throw new InvalidOperationException(
                $"HeaderInfo не зарегистрирован для модели {modelType.Name}");

        return ActivatorUtilities.CreateInstance<ProjectToolViewModel>(_sp, header, model);
    }
}

