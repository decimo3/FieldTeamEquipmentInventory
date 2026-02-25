using Microsoft.Data.Sqlite;

using FieldTeamEquipmentInventory.Interfaces;

namespace FieldTeamEquipmentInventory.Services;

public class SQLiteDB : Database, IDatabase
{
    private bool _disposed = false;
    public SQLiteDB() : base(
        new SqliteConnection(
            System.Environment.GetEnvironmentVariable("DNS_STRING")))
    {
        base.connection.Open();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources here.
                base.connection?.Dispose();
            }
            // Dispose unmanaged resources here.
            _disposed = true;
        }
    }
    ~SQLiteDB()
    {
        // Destructor (optional)
        Dispose(false);
    }

}