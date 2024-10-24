using FsmModules.Model;
using FsmModules.WaterSupply.Model.Entities;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using FsmModules.WaterSupply.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

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
    
    [ObservableProperty]
    private bool _selectAllRooms;
    
    [ObservableProperty]
    private string _filterByName = "";

    private const string RoomNameMissing = "Имя помещения отсутствует";

    internal WaterSupplyViewModel(Document doc)
    {
        _doc = doc;
        loadRooms();
        loadCategory();
    }

    private void loadRooms()
    {
        List<Room> rooms = new();
        var fixtures = new FilteredElementCollector(_doc)
            .OfCategory(BuiltInCategory.OST_PlumbingFixtures)
            .WhereElementIsNotElementType()
            .ToElements();
        foreach (FamilyInstance fixture in fixtures)
        {
            var room = fixture.Room;
            
            if (room == null)
                continue;
            if (rooms.Any(r => r.Id.IntegerValue == room.Id.IntegerValue))
                continue;
            if (room.Name.ToLower().Contains("кухня")) continue;
            if (room.Name.ToLower().Contains("пуи")) continue;
            if (room.Name.ToLower().Contains("бкфн")) continue;
            if (room.Name.Contains(_filterByName))
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
            .Where(i => i.CategoryType == CategoryType.Model)
            .Select(c => c.Name)
            .ToList();
    }

    private List<string> getSelectedRooms()
    {
        return _rooms.Where(r => r.IsChecked).Select(r => r.RoomName).ToList();
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

    partial void OnSelectAllRoomsChanged(bool value)
    {
        foreach (var room in Rooms)
        {
            room.IsChecked = value;
        }
        OnPropertyChanged(nameof(Rooms));
    }
    
    partial void OnFilterByNameChanged(string value)
    {
        loadRooms();
        OnPropertyChanged(nameof(Rooms));
    }
}
