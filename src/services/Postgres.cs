using FieldTeamEquipmentInventory.Interfaces;
using FieldTeamEquipmentInventory.Services;
using Npgsql;

namespace FieldTeamEquipmentInventory.Services;

public class Postgres : Database, IDatabase
{
    private bool _disposed = false;
    public Postgres() : base(
        new Npgsql.NpgsqlConnection(
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
    ~Postgres()
    {
        // Destructor (optional)
        Dispose(false);
    }
}
