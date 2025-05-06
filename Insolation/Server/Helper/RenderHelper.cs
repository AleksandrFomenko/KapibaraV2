using Autodesk.Revit.DB.DirectContext3D;
using Insolation.Server.Buffers;

namespace Insolation.Server.Helper;

public static class RenderHelper
{
    public static void MapSurfaceBuffer(RenderingBufferStorage buffer, Mesh mesh, double offset)
    {
        var vertexCount = mesh.Vertices.Count;
        var triangleCount = mesh.NumTriangles;

        buffer.VertexBufferCount = vertexCount;
        buffer.PrimitiveCount = triangleCount;

        var vertexBufferSizeInFloats = VertexPosition.GetSizeInFloats() * buffer.VertexBufferCount;
        buffer.FormatBits = VertexFormatBits.Position;
        buffer.VertexBuffer = new VertexBuffer(vertexBufferSizeInFloats);
        buffer.VertexBuffer.Map(vertexBufferSizeInFloats);

        var vertexStream = buffer.VertexBuffer.GetVertexStreamPosition();
        var normals = new List<XYZ>(mesh.NumberOfNormals);

        for (var i = 0; i < mesh.Vertices.Count; i++)
        {
            var normal = RenderGeometryHelper.GetMeshVertexNormal(mesh, i, mesh.DistributionOfNormals);
            normals.Add(normal);
        }

        for (var i = 0; i < mesh.Vertices.Count; i++)
        {
            var vertex = mesh.Vertices[i];
            var normal = normals[i];
            var offsetVertex = vertex + normal * offset;
            var vertexPosition = new VertexPosition(offsetVertex);
            vertexStream.AddVertex(vertexPosition);
        }

        buffer.VertexBuffer.Unmap();
        buffer.IndexBufferCount = triangleCount * IndexTriangle.GetSizeInShortInts();
        buffer.IndexBuffer = new IndexBuffer(buffer.IndexBufferCount);
        buffer.IndexBuffer.Map(buffer.IndexBufferCount);

        var indexStream = buffer.IndexBuffer.GetIndexStreamTriangle();

        for (var i = 0; i < triangleCount; i++)
        {
            var meshTriangle = mesh.get_Triangle(i);
            var index0 = (int) meshTriangle.get_Index(0);
            var index1 = (int) meshTriangle.get_Index(1);
            var index2 = (int) meshTriangle.get_Index(2);
            indexStream.AddTriangle(new IndexTriangle(index0, index1, index2));
        }

        buffer.IndexBuffer.Unmap();
        buffer.VertexFormat = new VertexFormat(buffer.FormatBits);
    }

    public static void MapCurveBuffer(RenderingBufferStorage buffer, IList<XYZ> vertices)
    {
        var vertexCount = vertices.Count;

        buffer.VertexBufferCount = vertexCount;
        buffer.PrimitiveCount = vertexCount - 1;

        var vertexBufferSizeInFloats = VertexPosition.GetSizeInFloats() * buffer.VertexBufferCount;
        buffer.FormatBits = VertexFormatBits.Position;
        buffer.VertexBuffer = new VertexBuffer(vertexBufferSizeInFloats);
        buffer.VertexBuffer.Map(vertexBufferSizeInFloats);

        var vertexStream = buffer.VertexBuffer.GetVertexStreamPosition();

        foreach (var vertex in vertices)
        {
            var vertexPosition = new VertexPosition(vertex);
            vertexStream.AddVertex(vertexPosition);
        }

        buffer.VertexBuffer.Unmap();
        buffer.IndexBufferCount = (vertexCount - 1) * IndexLine.GetSizeInShortInts();
        buffer.IndexBuffer = new IndexBuffer(buffer.IndexBufferCount);
        buffer.IndexBuffer.Map(buffer.IndexBufferCount);

        var indexStream = buffer.IndexBuffer.GetIndexStreamLine();

        for (var i = 0; i < vertexCount - 1; i++)
        {
            indexStream.AddLine(new IndexLine(i, i + 1));
        }

        buffer.IndexBuffer.Unmap();
        buffer.VertexFormat = new VertexFormat(buffer.FormatBits);
    }

    public static void MapCurveBuffer(RenderingBufferStorage buffer, IList<XYZ> vertices, double diameter)
    {
        var tubeSegments = RenderGeometryHelper.GetSegmentationTube(vertices, diameter);
        var segmentVerticesCount = tubeSegments[0].Count;
        var newVertexCount = vertices.Count * segmentVerticesCount;

        buffer.VertexBufferCount = newVertexCount;
        buffer.PrimitiveCount = (vertices.Count - 1) * segmentVerticesCount * 4;

        var vertexBufferSizeInFloats = VertexPosition.GetSizeInFloats() * buffer.VertexBufferCount;
        buffer.FormatBits = VertexFormatBits.Position;
        buffer.VertexBuffer = new VertexBuffer(vertexBufferSizeInFloats);
        buffer.VertexBuffer.Map(vertexBufferSizeInFloats);

        var vertexStream = buffer.VertexBuffer.GetVertexStreamPosition();

        foreach (var segment in tubeSegments)
        {
            foreach (var point in segment)
            {
                var vertexPosition = new VertexPosition(point);
                vertexStream.AddVertex(vertexPosition);
            }
        }

        buffer.VertexBuffer.Unmap();

        buffer.IndexBufferCount = (vertices.Count - 1) * segmentVerticesCount * 4 * IndexLine.GetSizeInShortInts();
        buffer.IndexBuffer = new IndexBuffer(buffer.IndexBufferCount);
        buffer.IndexBuffer.Map(buffer.IndexBufferCount);

        var indexStream = buffer.IndexBuffer.GetIndexStreamLine();

        for (var i = 0; i < vertices.Count - 1; i++)
        {
            for (var j = 0; j < segmentVerticesCount; j++)
            {
                var currentStart = i * segmentVerticesCount + j;
                var nextStart = (i + 1) * segmentVerticesCount + j;
                var currentEnd = i * segmentVerticesCount + (j + 1) % segmentVerticesCount;
                var nextEnd = (i + 1) * segmentVerticesCount + (j + 1) % segmentVerticesCount;

                // First triangle
                indexStream.AddLine(new IndexLine(currentStart, nextStart));
                indexStream.AddLine(new IndexLine(nextStart, nextEnd));

                // Second triangle
                indexStream.AddLine(new IndexLine(nextEnd, currentEnd));
                indexStream.AddLine(new IndexLine(currentEnd, currentStart));
            }
        }

        buffer.IndexBuffer.Unmap();
        buffer.VertexFormat = new VertexFormat(buffer.FormatBits);
    }

    public static void MapLinesBuffer(RenderingBufferStorage buffer, IList<Line> lines, double diameter)
        {
            var vertexList = new List<XYZ>();
            var indexList = new List<IndexLine>();
            int offset = 0;
            foreach (var line in lines)
            {
                var pts = line.Tessellate();
                if (pts == null || pts.Count < 2) continue;
                var tube = RenderGeometryHelper.GetSegmentationTube(pts, diameter);
                foreach (var ring in tube)
                {
                    foreach (var p in ring)
                    {
                        vertexList.Add(p);
                    }
                }
                int segCount = tube[0].Count;
                int segs = tube.Count;
                for (int s = 0; s < segs - 1; s++)
                {
                    for (int i = 0; i < segCount; i++)
                    {
                        int a = offset + s * segCount + i;
                        int b = offset + (s + 1) * segCount + i;
                        indexList.Add(new IndexLine(a, b));
                    }
                }
                offset += segCount * segs;
            }

            int vCount = vertexList.Count;
            buffer.VertexBufferCount = vCount;
            buffer.PrimitiveCount = indexList.Count;
            int floatsCount = VertexPosition.GetSizeInFloats() * vCount;
            buffer.FormatBits = VertexFormatBits.Position;
            buffer.VertexBuffer = new VertexBuffer(floatsCount);
            buffer.VertexBuffer.Map(floatsCount);
            var vstream = buffer.VertexBuffer.GetVertexStreamPosition();
            foreach (var v in vertexList)
                vstream.AddVertex(new VertexPosition(v));
            buffer.VertexBuffer.Unmap();

            int idxLen = indexList.Count * IndexLine.GetSizeInShortInts();
            buffer.IndexBufferCount = idxLen;
            buffer.IndexBuffer = new IndexBuffer(idxLen);
            buffer.IndexBuffer.Map(idxLen);
            var istream = buffer.IndexBuffer.GetIndexStreamLine();
            foreach (var idx in indexList)
                istream.AddLine(idx);
            buffer.IndexBuffer.Unmap();
            buffer.VertexFormat = new VertexFormat(buffer.FormatBits);
        }
    


    public static void MapCurveSurfaceBuffer(RenderingBufferStorage buffer, IList<XYZ> vertices, double diameter)
    {
        var tubeSegments = RenderGeometryHelper.GetSegmentationTube(vertices, diameter);
        var segmentVerticesCount = tubeSegments[0].Count;
        var newVertexCount = vertices.Count * segmentVerticesCount;

        buffer.VertexBufferCount = newVertexCount;
        buffer.PrimitiveCount = (vertices.Count - 1) * segmentVerticesCount * 2;

        var vertexBufferSizeInFloats = VertexPosition.GetSizeInFloats() * buffer.VertexBufferCount;
        buffer.FormatBits = VertexFormatBits.Position;
        buffer.VertexBuffer = new VertexBuffer(vertexBufferSizeInFloats);
        buffer.VertexBuffer.Map(vertexBufferSizeInFloats);

        var vertexStream = buffer.VertexBuffer.GetVertexStreamPosition();

        foreach (var segment in tubeSegments)
        {
            foreach (var point in segment)
            {
                var vertexPosition = new VertexPosition(point);
                vertexStream.AddVertex(vertexPosition);
            }
        }

        buffer.VertexBuffer.Unmap();

        buffer.IndexBufferCount = (vertices.Count - 1) * segmentVerticesCount * 6 * IndexTriangle.GetSizeInShortInts();
        buffer.IndexBuffer = new IndexBuffer(buffer.IndexBufferCount);
        buffer.IndexBuffer.Map(buffer.IndexBufferCount);

        var indexStream = buffer.IndexBuffer.GetIndexStreamTriangle();

        for (var i = 0; i < vertices.Count - 1; i++)
        {
            for (var j = 0; j < segmentVerticesCount; j++)
            {
                var currentStart = i * segmentVerticesCount + j;
                var nextStart = (i + 1) * segmentVerticesCount + j;
                var currentEnd = i * segmentVerticesCount + (j + 1) % segmentVerticesCount;
                var nextEnd = (i + 1) * segmentVerticesCount + (j + 1) % segmentVerticesCount;

                // First triangle
                indexStream.AddTriangle(new IndexTriangle(currentStart, nextStart, nextEnd));

                // Second triangle
                indexStream.AddTriangle(new IndexTriangle(nextEnd, currentEnd, currentStart));
            }
        }

        buffer.IndexBuffer.Unmap();
        buffer.VertexFormat = new VertexFormat(buffer.FormatBits);
    }
    public static void MapNormalVectorBuffer(RenderingBufferStorage buffer, XYZ origin, XYZ vector, double length)
    {
        var headSize = length > 1 ? 0.2 : length * 0.2;

        var endPoint = origin + vector * length;
        var arrowHeadBase = endPoint - vector * headSize;
        var basisVector = Math.Abs(vector.Z).IsAlmostEqual(1) ? XYZ.BasisY : XYZ.BasisZ;
        var perpendicular1 = vector.CrossProduct(basisVector).Normalize().Multiply(headSize * 0.5);

        buffer.VertexBufferCount = 4;
        buffer.PrimitiveCount = 3;

        var vertexBufferSizeInFloats = 4 * VertexPosition.GetSizeInFloats();
        buffer.FormatBits = VertexFormatBits.Position;
        buffer.VertexBuffer = new VertexBuffer(vertexBufferSizeInFloats);
        buffer.VertexBuffer.Map(vertexBufferSizeInFloats);

        var vertexStream = buffer.VertexBuffer.GetVertexStreamPosition();
        vertexStream.AddVertex(new VertexPosition(origin));
        vertexStream.AddVertex(new VertexPosition(endPoint));
        vertexStream.AddVertex(new VertexPosition(arrowHeadBase + perpendicular1));
        vertexStream.AddVertex(new VertexPosition(arrowHeadBase - perpendicular1));

        buffer.VertexBuffer.Unmap();
        buffer.IndexBufferCount = 3 * IndexLine.GetSizeInShortInts();
        buffer.IndexBuffer = new IndexBuffer(buffer.IndexBufferCount);
        buffer.IndexBuffer.Map(buffer.IndexBufferCount);

        var indexStream = buffer.IndexBuffer.GetIndexStreamLine();
        indexStream.AddLine(new IndexLine(0, 1));
        indexStream.AddLine(new IndexLine(1, 2));
        indexStream.AddLine(new IndexLine(1, 3));

        buffer.IndexBuffer.Unmap();
        buffer.VertexFormat = new VertexFormat(buffer.FormatBits);
    }
}