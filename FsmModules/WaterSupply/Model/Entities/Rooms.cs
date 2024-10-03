using CommunityToolkit.Mvvm.ComponentModel;

namespace FsmModules.WaterSupply.Model.Entities
{
    public partial class Rooms : ObservableObject
    {
        [ObservableProperty]
        private bool isChecked;

        public string RoomName { get; set; }
        public int RoomId { get; set; }
    }
}