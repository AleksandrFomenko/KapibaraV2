namespace FsmModules.Storage;

internal interface IDataBase
{
    internal void ConnectDb(string databasePath);
    internal void CreateDatabase();
    internal void AddType();
    internal void getAllTypes();
}