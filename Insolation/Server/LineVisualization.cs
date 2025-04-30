using Autodesk.Revit.DB.DirectContext3D;
using Autodesk.Revit.DB.ExternalService;
using Insolation.Server.Buffers;

namespace Insolation.Server;

public class LineVisualization : IDirectContext3DServer
{
    private readonly Guid _guid = Guid.NewGuid();
    
    private bool _drawSurface;
    private double _transparency;
    private double _diameter;
    
    private readonly RenderingBufferStorage _curveBuffer = new();
    
    
    public Guid GetServerId() => _guid;
    public ExternalServiceId GetServiceId() => ExternalServices.BuiltInExternalServices.DirectContext3DService;
    public string GetName() => "Line visualization";
    public string GetVendorId() => "123";
    public string GetDescription() => "Line visualization";
    public bool CanExecute(View dBView) => true;
    public string GetApplicationId() =>  string.Empty;
    public string GetSourceId() => string.Empty;
    public bool UsesHandles() => false;
    public Outline GetBoundingBox(View dBView) => null;
    public bool UseInTransparentPass(View dBView) => _drawSurface && _transparency > 0;
    
    public void RenderScene(View dBView, DisplayStyle displayStyle)
    {
        
    }
}