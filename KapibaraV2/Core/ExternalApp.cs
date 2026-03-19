﻿using System.ComponentModel;
using Autodesk.Revit.UI;
#if NETCOREAPP
using System.Runtime.Loader;
#endif
namespace KapibaraV2.Core;

public abstract class ExternalApp : IExternalApplication
{

    public Result Result { get; set; } = Result.Succeeded;

    public UIControlledApplication Application { get; private set; } = null!;

    
    public UIApplication UiApplication => Context.UiApplication;

   
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Result OnStartup(UIControlledApplication application)
    {
        Application = application;

        try
        {
            var currentType = GetType();
#if NETCOREAPP
            if (AssemblyLoadContext.GetLoadContext(currentType.Assembly) == AssemblyLoadContext.Default)
            {
                ResolveHelper.BeginAssemblyResolve(currentType);
            }
#else
            ResolveHelper.BeginAssemblyResolve(currentType);

#endif
            OnStartup();
        }
        finally
        {
            ResolveHelper.EndAssemblyResolve();
        }

        return Result;
    }
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Result OnShutdown(UIControlledApplication application)
    {
        try
        {
            var currentType = GetType();
#if NETCOREAPP
            if (AssemblyLoadContext.GetLoadContext(currentType.Assembly) == AssemblyLoadContext.Default)
            {
                ResolveHelper.BeginAssemblyResolve(currentType);
            }
#else
            ResolveHelper.BeginAssemblyResolve(currentType);

#endif
            OnShutdown();
        }
        finally
        {
            ResolveHelper.EndAssemblyResolve();
        }

        return Result.Succeeded;
    }


    public abstract void OnStartup();


    public virtual void OnShutdown()
    {
    }
}