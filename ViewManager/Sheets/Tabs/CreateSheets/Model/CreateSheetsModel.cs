namespace ViewManager.Sheets.Tabs.CreateSheets.Model;

internal class CreateSheetsModel
{
    internal CreateSheetsData _data;
    internal CreateSheetsModel(Document doc)
    {
        _data = new CreateSheetsData(doc);
    }
    
}

