﻿using System.Collections.ObjectModel;
using LegendPlacer.Models;

namespace LegendPlacer.Services
{
    public class SheetOrganizationService(Document doc)
    {
        private readonly Document _doc = doc ?? throw new ArgumentNullException(nameof(doc));
        internal List<SheetItem> GetCheckedSheets(IEnumerable<FolderItem> folders)
        {
            var result = new List<SheetItem>();
            foreach (var folder in folders)
            {
                result.AddRange(folder.Sheets.Where(s => s.IsChecked));
                result.AddRange(GetCheckedSheets(folder.SubFolders));
            }
            return result;
        }
        internal ObservableCollection<FolderItem> GetSheetOrganization()
    {
        var result = new ObservableCollection<FolderItem>();
        var bo = BrowserOrganization.GetCurrentBrowserOrganizationForSheets(_doc);
        var sheets = new FilteredElementCollector(_doc)
            .OfClass(typeof(ViewSheet))
            .WhereElementIsNotElementType()
            .ToElements();
        foreach (var sheet in sheets)
        {
            var folderChain = bo.GetFolderItems(sheet.Id);
            var currentCollection = result;
            FolderItem currentFolder = null;
            foreach (var folder in folderChain)
            {
                var foundFolder = currentCollection.FirstOrDefault(f => f.Name == folder.Name);
                if (foundFolder == null)
                {
                    foundFolder = new FolderItem { Name = folder.Name };
                    currentCollection.Add(foundFolder);
                }

                currentFolder = foundFolder;
                currentCollection = foundFolder.SubFolders;
            }

            if (currentFolder != null)
            {
                if (sheet is ViewSheet viewSheet)
                {
                    var sheetItem = new SheetItem(
                        viewSheet.Name,
                        viewSheet.SheetNumber,
                        viewSheet.Id.IntegerValue
                    );
                    currentFolder.Sheets.Add(sheetItem);
                }
            }
            else
            {
                var unorganizedFolder = result.FirstOrDefault(f => f.Name == "err");
                if (unorganizedFolder == null)
                {
                    unorganizedFolder = new FolderItem { Name = "err" };
                    result.Add(unorganizedFolder);
                }

                if (sheet is ViewSheet viewSheet)
                {
                    var sheetItem = new SheetItem(
                        viewSheet.Name,
                        viewSheet.SheetNumber,
                        viewSheet.Id.IntegerValue
                    );
                    unorganizedFolder.Sheets.Add(sheetItem);
                }
            }
        }
        return result;
    } 
    }
}
