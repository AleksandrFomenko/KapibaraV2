using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;
using Autodesk.Revit.DB.ExternalService;
using Insolation.Server.Buffers;
using Insolation.Server.Helper;
using JetBrains.Annotations;

namespace Insolation.Server;

public class LineVisualization : IDirectContext3DServer
{
    private readonly Guid _guid = Guid.NewGuid();
    
    private bool _hasEffectsUpdates = true;
    private bool _hasGeometryUpdates = true;
    private bool _drawSurface = true;
    private double _transparency;
    private double _diameter;
    private bool _drawCurve = true;
    private IList<XYZ> _vertices = null!;
    private List<Line> _lines = null!;
    private readonly object _renderLock = new();
    
   // private readonly RenderingBufferStorage _surfaceBuffer = new();
    //private readonly RenderingBufferStorage _curveBuffer = new();
    private readonly RenderingBufferStorage _linesBuffer = new();
    private readonly RenderingBufferStorage _mapLinesSurfaceBuffer = new();

    private readonly List<RenderingBufferStorage> _normalsBuffers = new(1);
    
    
    public Guid GetServerId() => _guid;
    public ExternalServiceId GetServiceId() => ExternalServices.BuiltInExternalServices.DirectContext3DService;
    public string GetName() => "Line visualization";
    public string GetVendorId() => "123";
    public string GetDescription() => "Line visualization";
    public bool CanExecute(View dBView) => true;
    public string GetApplicationId() =>  string.Empty;
    public string GetSourceId() => string.Empty;
    public bool UsesHandles() => false;
    [CanBeNull] public Outline GetBoundingBox(View dBView) => null;
    public bool UseInTransparentPass(View dBView) => _drawSurface && _transparency > 0;
    
     public void RenderScene(View view, DisplayStyle displayStyle)
    {
        lock (_renderLock)
        {
            try
            {
                // if (_hasGeometryUpdates || !_surfaceBuffer.IsValid() || !_linesBuffer.IsValid())
                if (_hasGeometryUpdates || !_linesBuffer.IsValid())
                {
                    MapGeometryBuffer();
                    _hasGeometryUpdates = false;
                }
                if (_hasEffectsUpdates)
                {
                    UpdateEffects();
                    _hasEffectsUpdates = false;
                }

                if (_drawSurface)
                {
                    var isTransparentPass = DrawContext.IsTransparentPass();
                    if (isTransparentPass && _transparency > 0 || !isTransparentPass && _transparency == 0)
                    {
                        /*
                        DrawContext.FlushBuffer(_surfaceBuffer.VertexBuffer,
                            _surfaceBuffer.VertexBufferCount,
                            _surfaceBuffer.IndexBuffer,
                            _surfaceBuffer.IndexBufferCount,
                            _surfaceBuffer.VertexFormat,
                            _surfaceBuffer.EffectInstance, PrimitiveType.TriangleList, 0,
                            _surfaceBuffer.PrimitiveCount);
                        */
                        DrawContext.FlushBuffer( _mapLinesSurfaceBuffer.VertexBuffer,
                            _mapLinesSurfaceBuffer.VertexBufferCount,
                            _mapLinesSurfaceBuffer.IndexBuffer,
                            _mapLinesSurfaceBuffer.IndexBufferCount,
                            _mapLinesSurfaceBuffer.VertexFormat,
                            _mapLinesSurfaceBuffer.EffectInstance, PrimitiveType.TriangleList, 0,
                            _mapLinesSurfaceBuffer.PrimitiveCount);

                    }
                }
                 /*
                if (_drawCurve)
                {
                    _curveBuffer.EffectInstance ??= new EffectInstance(_curveBuffer.FormatBits);
                    _linesBuffer.EffectInstance ??= new EffectInstance(_linesBuffer.FormatBits);
                    DrawContext.FlushBuffer(_curveBuffer.VertexBuffer,
                        _curveBuffer.VertexBufferCount,
                        _curveBuffer.IndexBuffer,
                        _curveBuffer.IndexBufferCount,
                        _curveBuffer.VertexFormat,
                        _curveBuffer.EffectInstance, PrimitiveType.LineList, 0,
                     _curveBuffer.PrimitiveCount);
                
                }
                */
                if (_lines.Count > 0 && _linesBuffer.IsValid())
                {
                    DrawContext.FlushBuffer(
                        _linesBuffer.VertexBuffer,
                        _linesBuffer.VertexBufferCount,
                        _linesBuffer.IndexBuffer,
                        _linesBuffer.IndexBufferCount,
                        _linesBuffer.VertexFormat,
                        _linesBuffer.EffectInstance,
                        PrimitiveType.LineList,
                        0,
                        _linesBuffer.PrimitiveCount);
                }
                
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
    
    public void Register(List<Line> lines)
    {
        _lines = lines;

        RevitShell.ActionEventHandler.Raise(application =>
        {
            if (application.ActiveUIDocument is null) return;

            var directContextService = (MultiServerService) ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DirectContext3DService);
            var serverIds = directContextService.GetActiveServerIds();

            directContextService.AddServer(this);
            serverIds.Add(GetServerId());
            directContextService.SetActiveServers(serverIds);

            application.ActiveUIDocument.UpdateAllOpenViews();
        });
    }
    
    private void MapGeometryBuffer()
    {
       // RenderHelper.MapCurveSurfaceBuffer(_surfaceBuffer, _vertices, 0.1);
        RenderHelper. MapLinesSurfaceBuffer(_mapLinesSurfaceBuffer,_lines, 0.025);
        //RenderHelper.MapCurveBuffer(_curveBuffer, _vertices, 0.025);
        if (_lines.Count > 0)
        {
            RenderHelper.MapLinesBuffer(_linesBuffer, _lines, 0.025);
        }

        //MapDirectionsBuffer();
    }
    
    private void UpdateEffects()
    {
        /*
        _surfaceBuffer.EffectInstance ??= new EffectInstance(_surfaceBuffer.FormatBits);
        _surfaceBuffer.EffectInstance.SetColor(new Color(0, 0, 255));
        _surfaceBuffer.EffectInstance.SetTransparency(_transparency);
        */
        _mapLinesSurfaceBuffer.EffectInstance ??= new EffectInstance(_mapLinesSurfaceBuffer.FormatBits);
        _mapLinesSurfaceBuffer.EffectInstance.SetColor(new Color(0, 255, 0));
        _mapLinesSurfaceBuffer.EffectInstance.SetTransparency(_transparency);
        
        
        _linesBuffer.EffectInstance ??= new EffectInstance(_linesBuffer.FormatBits);
        _linesBuffer.EffectInstance.SetColor(new Color(0, 250, 0));
        _linesBuffer.EffectInstance.SetTransparency(_transparency);
       
        foreach (var buffer in _normalsBuffers)
        {
            buffer.EffectInstance ??= new EffectInstance(buffer.FormatBits);
            buffer.EffectInstance.SetColor(new Color(0, 0, 255));
        }
    }
    
    private void MapDirectionsBuffer()
    {
        var verticalOffset = 0d;

        for (var i = 0; i < _vertices.Count - 1; i++)
        {
            var startPoint = _vertices[i];
            var endPoint = _vertices[i + 1];
            var centerPoint = (startPoint + endPoint) / 2;
            var buffer = CreateNormalBuffer(i);

            var segmentVector = endPoint - startPoint;
            var segmentLength = segmentVector.GetLength();
            var segmentDirection = segmentVector.Normalize();
            if (verticalOffset == 0)
            {
                verticalOffset = RenderGeometryHelper.InterpolateOffsetByDiameter(_diameter) + _diameter / 2d;
            }

            var offsetVector = XYZ.BasisX.CrossProduct(segmentDirection).Normalize() * verticalOffset;
            if (offsetVector.IsZeroLength())
            {
                offsetVector = XYZ.BasisY.CrossProduct(segmentDirection).Normalize() * verticalOffset;
            }

            if (offsetVector.Z < 0)
            {
                offsetVector = -offsetVector;
            }

            var arrowLength = segmentLength > 1 ? 1d : segmentLength * 0.6;
            var arrowOrigin = centerPoint + offsetVector - segmentDirection * (arrowLength / 2);

            RenderHelper.MapNormalVectorBuffer(buffer, arrowOrigin, segmentDirection, arrowLength);
        }
    }
    
    private RenderingBufferStorage CreateNormalBuffer(int vertexIndex)
    {
        RenderingBufferStorage buffer;
        if (_normalsBuffers.Count > vertexIndex)
        {
            buffer = _normalsBuffers[vertexIndex];
        }
        else
        {
            buffer = new RenderingBufferStorage();
            _normalsBuffers.Add(buffer);
        }

        return buffer;
    }

    public void Unregister()
    {
        RevitShell.ActionEventHandler.Raise(application =>
        {
            var directContextService = (MultiServerService) ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DirectContext3DService);
            directContextService.RemoveServer(GetServerId());

            application.ActiveUIDocument?.UpdateAllOpenViews();
        });
    }
}