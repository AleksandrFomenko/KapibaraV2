namespace RiserMate.Abstractions;

public interface IViewCreationService
{
    public View3D CreateView3D(string parameterName, string name, string viewTypeName);
}