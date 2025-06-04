using ClashDetective.Structure;

namespace ClashDetective.ViewModels.Format;

public interface IFormat;

public interface IFormat<TItem>
{
    string Name { get; }
    IEnumerable<TItem> Parse(string filePath);
}