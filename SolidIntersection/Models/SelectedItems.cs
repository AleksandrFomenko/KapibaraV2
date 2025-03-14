using System.Collections;
using Visibility = System.Windows.Visibility;

namespace SolidIntersection.Models
{
    public partial class SelectedItems : ObservableObject
    {
        [ObservableProperty] private string _nameItem;
        [ObservableProperty] private string _value;
        [ObservableProperty] private bool _isChecked;
        [ObservableProperty] private Visibility _visibleTextBox = Visibility.Visible;

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