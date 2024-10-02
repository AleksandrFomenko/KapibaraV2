using FsmModules.Model;
using FsmModules.WaterSupply.Model.Entities;
using Autodesk.Revit.DB.Architecture;
using FsmModules.WaterSupply.Model;

namespace FsmModules.WaterSupply.ViewModel;

public partial class WaterSupplyViewModel : ObservableObject, IWorker
{
    private Document _doc;

    [ObservableProperty]
    private List<Rooms> _rooms = new();
    
    [ObservableProperty]
    private Rooms _selectedRoom;
    
    [ObservableProperty]
    private List<string> _categories;
    
    [ObservableProperty]
    private string _selectedCategory;
    


    private const string RoomNameMissing = "Имя помещения отсутствует";

    internal WaterSupplyViewModel(Document doc)
    {

        _doc = doc;
        loadRooms();
        loadCategory();
    }

    private void loadRooms()
    {
        var rooms = new List<Room>();
        var fixtures = new FilteredElementCollector(_doc)
            .OfCategory(BuiltInCategory.OST_PlumbingFixtures)
            .WhereElementIsNotElementType()
            .ToElements();
        foreach (FamilyInstance fixture in fixtures)
        {
            var room = fixture.Room;
            if (!rooms.Contains(room))
            {
                rooms.Add(room);
            }
        }

        _rooms = rooms
            .Select(f => new Rooms
            {
                IsChecked = false,
                RoomName = f?.Name ?? RoomNameMissing,
                RoomId = f?.Id?.IntegerValue ?? 0
            })
            .ToList();
    }

    private void loadCategory()
    {
        var categories = _doc.Settings.Categories;
        _categories = categories
            .Cast<Category>()
            .Where(i => i.IsVisibleInUI)
            .Where(i => i.AllowsBoundParameters)
            .Where((i => i.CategoryType == CategoryType.Model))
            .Select(c => c.Name)
            .ToList();

    }

    private List<string> getSelectedRooms()
    {
        return _rooms.Where(r => r.IsChecked).Select(r=>r.RoomName).ToList();
    }


    public void Start()
    {
        WaterSupplyModel vsp = new WaterSupplyModel(_doc);
        var roomNames = getSelectedRooms();
        var t = new Transaction(_doc, "Create water supply modules");
        t.Start();
        foreach (var roomName in roomNames)
        {
            var roomElement = new FilteredElementCollector(_doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .Cast<Room>()
                .FirstOrDefault(r => r.Name == roomName);
            vsp.createDirectShape(roomElement, _selectedCategory);
        }

        t.Commit();
    }
}