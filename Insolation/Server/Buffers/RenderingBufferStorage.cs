using Autodesk.Revit.DB.DirectContext3D;
 
namespace Insolation.Server.Buffers;

public sealed class RenderingBufferStorage
{
    public VertexFormatBits FormatBits { get; set; }
    public int PrimitiveCount { get; set; }
    public int VertexBufferCount { get; set; }
    public int IndexBufferCount { get; set; }
    public VertexBuffer? VertexBuffer { get; set; }
    public IndexBuffer? IndexBuffer { get; set; }
    public VertexFormat? VertexFormat { get; set; }
    public EffectInstance? EffectInstance { get; set; }

    public bool IsValid()
    {
        if (VertexBuffer is null || !VertexBuffer.IsValid()) return false;
        if (IndexBuffer is null || !IndexBuffer.IsValid()) return false;
        if (VertexFormat is null || !VertexFormat.IsValid()) return false;
        if (EffectInstance is null || !EffectInstance.IsValid()) return false;

        return true;
    }
}