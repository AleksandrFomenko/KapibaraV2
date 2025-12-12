using KapibaraCore.Parameters;
using RiserMate.Abstractions;
using RiserMate.Core;
using RiserMate.Entities;
using RiserMate.Implementation;


namespace RiserMate.Models.Revit;

public class ModelRiserMateCreator(
    IViewCreationService viewCreationService,
    IFilterCreationService filterCreationService) : IModelRiserCreator
{
    private readonly Document? _document = Context.ActiveDocument;

    public List<string> GetUserParameters()
    {
        return _document.GetProjectParameters(BuiltInCategory.OST_PipeCurves);
    }

    public List<HeatingRiser> GetHeatingRisers(string parameter)
    {
        var pipes = new FilteredElementCollector(_document)
            .OfCategory(BuiltInCategory.OST_PipeCurves)
            .WhereElementIsNotElementType()
            .ToElements();

        var hashSet = new HashSet<string>();

        foreach (var pipe in pipes)
        {
            var rise = GetParameterValue(pipe, parameter);
            if (rise == string.Empty) continue;
            hashSet.Add(GetParameterValue(pipe, parameter));
        }

        return hashSet.Select(p => new HeatingRiser(p)).ToList();
    }

    public List<string> GetMarksHeatDevice()
    {
        return GetMarks(BuiltInCategory.OST_MechanicalEquipmentTags);
    }

    public List<string> GetMarksPipe()
    {
        return GetMarks(BuiltInCategory.OST_PipeTags);
    }

    public List<string> GetMarksPipeAccessory()
    {
        return GetMarks(BuiltInCategory.OST_PipeAccessoryTags);
    }


    private List<string> GetMarks<T>(T category) where T : Enum
    {
        if (typeof(T) != typeof(BuiltInCategory))
            throw new ArgumentException("Only BuiltInCategory is supported.");

        var builtInCategory = (BuiltInCategory)(object)category;

        var marks = new FilteredElementCollector(_document)
            .OfCategory(builtInCategory)
            .WhereElementIsElementType()
            .ToElements();

        return marks
            .Select(e => e.Name)
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();
    }


    public List<string> GetTypes3D()
    {
        var types = new FilteredElementCollector(Context.ActiveDocument)
            .OfClass(typeof(ViewFamilyType))
            .Cast<ViewFamilyType>()
            .Where(v => v.ViewFamily == ViewFamily.ThreeDimensional)
            .Where(v => v.DefaultTemplateId == ElementId.InvalidElementId)
            .Select(v => v.Name)
            .ToList();
        return types.Count == 0 ? ["Тип 3D вида не найден"] : types;
    }

    private static string GetParameterValue(Element elem, string parameterName)
    {
        return elem.LookupParameter(parameterName)?.AsString() ?? string.Empty;
    }

    public void SelectHeatingRiser(HeatingRiser e, string parameter)
    {
        var elements = new FilteredElementCollector(_document)
            .WhereElementIsNotElementType()
            .Where(pipe => pipe.LookupParameter(parameter)?.AsString() == e.Name)
            .ToList();

        var sel = Context.ActiveUiDocument?.Selection;

        sel?.SetElementIds(elements.Select(x => x.Id).ToList());
    }

    public void Show3D(HeatingRiser e)
    {
        Console.WriteLine("Show3D");
    }

    public async Task ExecuteAsync(List<HeatingRiser> heatingRisers, string parameter,
        IProgress<(int val, string msg)> progress)
    {
        await Handlers.Handlers.AsyncEventHandler.RaiseAsync(async app =>
        {
            using var t = new Transaction(_document, "RiserMate: Обработка стояков");

            try
            {
                t.Start();

                for (var i = 0; i < heatingRisers.Count; i++)
                {
                    var riser = heatingRisers[i];
                    progress?.Report((i + 1, $"Обработка: {riser.Name}"));

                    try
                    {
                        var pip = RiserMateCore.GetBottomPipesByRiser(riser.Name, parameter);
                        if (pip != null) RiserMateCore.Execute(pip, riser.Name, parameter);
                    }
                    catch (Exception localEx)
                    {
                        Console.WriteLine($"Ошибка на стояке {riser.Name}: {localEx.Message}");
                    }
                }

                t.Commit();
            }
            catch (Exception ex)
            {
                if (t.GetStatus() == TransactionStatus.Started) t.RollBack();

                throw new Exception($"Критическая ошибка транзакции: {ex.Message}", ex);
            }
        });
    }

    public async Task CreateViewsAsync(
        List<HeatingRiser> heatingRisers,
        string parameterName,
        string viewOption,
        bool isMarking,
        string marksHeatDevice,
        string marksPipe,
        string markPipeAccessory,
        IProgress<(int val, string msg)> progress)
    {
        await Handlers.Handlers.AsyncEventHandler.RaiseAsync(async app =>
        {
            using var t = new Transaction(_document, "RiserMateCreateViews");

            try
            {
                t.Start();

                for (var i = 0; i < heatingRisers.Count; i++)
                {
                    var heatingRiser = heatingRisers[i];

                    progress?.Report((i + 1, $"Создание вида: {heatingRiser.Name}"));


                    var view = viewCreationService.CreateView3D(parameterName, heatingRiser.Name, viewOption);
                    if (view == null) continue;


                    var filter = filterCreationService.CreateFilter(parameterName, heatingRiser.Name);
                    if (filter != null)
                    {
                        view.AddFilter(filter.Id);
                        view.SetFilterVisibility(filter.Id, false);
                    }

                    _document?.Regenerate();

                    
                    if (isMarking)
                    {
                        var service = new LabelingService(view);
                        try
                        {
                            if (!string.IsNullOrEmpty(marksHeatDevice))
                                service.MarkHeatDevice(marksHeatDevice);

                            if (!string.IsNullOrEmpty(marksPipe))
                                service.MarkPipe(marksPipe);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Ошибка маркировки {heatingRiser.Name}: {e.Message}");
                        }
                    }
                }

                t.Commit();
            }
            catch (Exception ex)
            {
                if (t.GetStatus() == TransactionStatus.Started) t.RollBack();
                throw;
            }
        });
    }

    public async void СreateViews(
        List<HeatingRiser> heatingRisers,
        string parameterName,
        string viewOption,
        bool isMarking,
        string marksHeatDevice,
        string marksPipe,
        string markPipeAccessory)
    {
        await Handlers.Handlers.AsyncEventHandler.RaiseAsync(async app =>
        {
            using var t = new Transaction(_document, "RiserMateCreateViews");
            t.Start();
            foreach (var heatingRiser in heatingRisers)
            {
                var view = viewCreationService.CreateView3D(parameterName, heatingRiser.Name, viewOption);
                var filter = filterCreationService.CreateFilter(parameterName, heatingRiser.Name);
                view.AddFilter(filter.Id);
                view.SetFilterVisibility(filter.Id, false);
                
                _document?.Regenerate();
                
                if (!isMarking) continue;
                var service = new LabelingService(view);
                try
                {
                    if (marksHeatDevice != null) service.MarkHeatDevice(marksHeatDevice);

                    if (marksPipe != null) service.MarkPipe(marksPipe);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            t.Commit();
        });
    }
}