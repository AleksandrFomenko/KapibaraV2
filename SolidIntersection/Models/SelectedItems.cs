using System.Collections;

namespace SolidIntersection.Models
{
    public partial class SelectedItems : ObservableObject
    {
        [ObservableProperty]
        private string _nameItem;

        [ObservableProperty]
        private bool _isChecked;

        public SelectedItems(string nameItem, bool isChecked)
        {
            _nameItem = nameItem;
            _isChecked = isChecked;
        }

        internal string GetName()
        {
            return NameItem;
        }
        internal void SetCheck(bool value)
        { 
            IsChecked = value;
        }
    }
}