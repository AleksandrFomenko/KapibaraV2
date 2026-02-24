using Marking.Entities;

namespace Marking.Abstractions;

public interface IMarkingModel
{
    public event EventHandler<CustomEventArgs> SendName;
    public event Action SendMaximize;
    public event Action<int>? SendQuantity;
    public int PickMark();
    public void SetSelectedChoice(Choice? choice);
    public Task Execute(int elementId);
}

public class CustomEventArgs(string name) : EventArgs 
{ 
    public string Name {get; set;} = name;
}