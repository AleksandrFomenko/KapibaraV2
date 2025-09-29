using ProjectAxes.Abstractions;
using ProjectAxes.Models;

namespace ProjectAxes.Factories;

public interface IViewModelFactory
{
    IViewModel Create<TModel>(TModel model)
        where TModel : class, IModel;
}
