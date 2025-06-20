using ClashHub.Models.Parsers;

namespace ClashHub.ViewModels.Format;

public class Format<TModel, TItem> : IFormat<TItem>
{
    public string Name { get; }
    private readonly IFileParser<TModel> _parser;
    private readonly Func<TModel, IEnumerable<TItem>> _extractor;

    public Format(string name, IFileParser<TModel> parser, Func<TModel, IEnumerable<TItem>> extractor)
    {
        Name       = name      ?? throw new ArgumentNullException(nameof(name));
        _parser    = parser    ?? throw new ArgumentNullException(nameof(parser));
        _extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
    }

    public IEnumerable<TItem> Parse(string filePath)
    {
        var model = _parser.Parse(filePath);
        return _extractor(model);
    }
}
