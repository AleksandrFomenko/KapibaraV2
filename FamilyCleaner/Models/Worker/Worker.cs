﻿using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using FamilyCleaner.Models.FamilyCleaning;
using FamilyCleaner.Models.Open;


namespace FamilyCleaner.Models.Worker;

public class Worker
{
    private string _path;
    private Application _app;
    private readonly FailureProcessor _failureProcessor;
    
    public Worker()
    {
        _failureProcessor = new FailureProcessor();
        _app = Context.Application;
    }

    public void Execute(string pathDownload, string pathSave)
    {
        _app.FailuresProcessing += _failureProcessor.ApplicationOnFailuresProcessing; 
        var doc  = Open.FamilyOpener.OpenFamily(pathDownload);
        
        var t = new Transaction(doc, "CleaningFamily");
        t.Start();
        
        CleaningManager.CleaningFamily(doc);
        
        t.Commit();
        doc.SaveAs(pathSave); 
        doc.Close(false); 
        
        
        _app.FailuresProcessing -= _failureProcessor.ApplicationOnFailuresProcessing;
    }
}