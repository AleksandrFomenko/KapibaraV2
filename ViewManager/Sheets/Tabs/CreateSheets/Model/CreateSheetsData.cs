namespace ViewManager.Sheets.Tabs.CreateSheets.Model;

internal class CreateSheetsData
{
    private Document _doc;
    
    internal CreateSheetsData(Document doc)
    {
        _doc = doc;
    }
    
    internal List<SheetsType> TitleBlocks()
    {
        var x = new FilteredElementCollector(_doc)
            .OfCategory(BuiltInCategory.OST_TitleBlocks)
            .WhereElementIsElementType()
            .ToElements();
        List<SheetsType> sheetsTypes = new List<SheetsType>();
        if (x.Any())
        {
            foreach (var element in x)
            {
                var familyNameParameter = element.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM);
                if (familyNameParameter == null || string.IsNullOrEmpty(familyNameParameter.AsString()))
                {
                    continue;
                }
                var typeNameParameter = element.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_NAME);
                if (typeNameParameter == null || string.IsNullOrEmpty(typeNameParameter.AsString()))
                {
                    continue;
                }

                var result = $"{familyNameParameter.AsString()} : {typeNameParameter.AsString()}";
                sheetsTypes.Add(new SheetsType() { Id = element.Id.IntegerValue, Name = result });
            }
        }
        return sheetsTypes;
    }
    
}