using System.Collections;

namespace SolidIntersector.Models
{
    public class SelectedItems : IEnumerable
    {
        public string NameItem { get; set; }
        public bool IsChecked { get; set; }
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}