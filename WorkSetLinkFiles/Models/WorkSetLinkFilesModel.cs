using System.Diagnostics;

namespace WorkSetLinkFiles.Models;

internal class WorkSetLinkFilesModel
{
    internal Data Data;
    private readonly Document _doc;
    


    internal WorkSetLinkFilesModel(Document doc)
    {
        _doc = doc;
        Data = new Data(_doc);
    }

    internal void SetLinksWorkset(List<LinkFiles> linkFilesList)
    {
        foreach (var link in linkFilesList)
        {
            var workset = Workset.Create(_doc, link.WorksetName);
            var linkModel = Data.GetLink(link.RevitModelName);
            Debug.Write(workset.Name);
            linkModel?.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM).Set(workset.Id.IntegerValue);
        }
    }
}