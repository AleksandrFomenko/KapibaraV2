using System.IO;
using System.Xml.Serialization;

namespace ClashHub.Models.Parsers.Xml;

public class XmlFileParser<T> : IFileParser<T>
{
    public T Parse(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Путь не задан", nameof(filePath));
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Файл не найден", filePath);

        var serializer = new XmlSerializer(typeof(T));
        using var stream = File.OpenRead(filePath);
        return (T)serializer.Deserialize(stream);
    }
}