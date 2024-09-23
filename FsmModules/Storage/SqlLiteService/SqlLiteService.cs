namespace FsmModules.Storage.SqlLiteService;
using Microsoft.Data.Sqlite;
using System;

internal class SqlLiteService : IDataBase
{
    private string _connectionString;
    public void ConnectDb(string databasePath)
    {
        throw new NotImplementedException();
    }

    public void CreateDatabase()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"
                CREATE TABLE IF NOT EXISTS WallTypes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    MainWall TEXT NOT NULL,
                    OtherWall TEXT NOT NULL,
                    Image BLOB
                );
            ";
            command.ExecuteNonQuery();
        }
    }

    public void AddType()
    {
        throw new NotImplementedException();
    }

    public void getAllTypes()
    {
        throw new NotImplementedException();
    }
}