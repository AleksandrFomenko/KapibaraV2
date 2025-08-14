namespace ClashHub.Models.Entity;

public class ElementEntity(string Type, int Id, string TypeFamily)
{
    public string Type { get; set; } = Type;
    public int Id { get; set; } = Id;
    public string TypeFamily { get; set; } = TypeFamily;
}