using System.Data;

using FieldTeamEquipmentInventory.Models;
using FieldTeamEquipmentInventory.Interfaces;

namespace FieldTeamEquipmentInventory.Services;

public abstract class Database : IDatabase
{
    protected readonly IDbConnection connection;
    protected Database(IDbConnection conn)
    {
        connection = conn;
    }
    
    private static void AddParameter(IDbCommand command, string name, object value)
    {
        var param = command.CreateParameter();
        param.ParameterName = name;
        param.Value = value ?? DBNull.Value;
        command.Parameters.Add(param);
    }

    public void AddEmployer(Employer employer)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO employers (id, fullname, biometric, created_at) VALUES (@id, @name, @biometric, @created)";
        AddParameter(command, "@id", employer.Registry);
        AddParameter(command, "@name", employer.FullName);
        AddParameter(command, "@biometric", employer.Template);
        AddParameter(command, "@created", employer.CreateAt);
        command.ExecuteNonQuery();
    }

    public Employer? GetEmployer(int id)
    {        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, fullname, biometric, created_at FROM employers WHERE id = @value LIMIT 1";
        AddParameter(command, "@value", id);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Employer
            (
                registry: reader.GetString(0),
                fullname: reader.GetString(1),
                template: reader.GetString(2),
                createAt: reader.GetDateTime(3)
            );
        }
        return null;
    }

    public List<Employer> GetEmployers()
    {
        var employers = new List<Employer>();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, fullname, biometric, created_at FROM employers";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            employers.Add(new Employer
            (
                registry: reader.GetString(0),
                fullname: reader.GetString(1),
                template: reader.GetString(2),
                createAt: reader.GetDateTime(3)
            ));
        }
        return employers;
    }
    public void AddEquipment(Equipment equipment)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO equipments (id, kind, status, kit) VALUES (@id, @kind, @status, @kit)";
        AddParameter(command, "@id", equipment.Id);
        AddParameter(command, "@kind", (int)equipment.Kind);
        AddParameter(command, "@status", (int)equipment.Status);
        AddParameter(command, "@kit", equipment.Kit);
        command.ExecuteNonQuery();
    }

    public Equipment? GetEquipment(long id)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, kind, status, kit FROM equipments WHERE id = @value LIMIT 1";
        AddParameter(command, "@value", id);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Equipment
            (
                id: reader.GetInt64(0),
                kind: (Equipment.KindEnum)reader.GetInt32(1),
                status: (Equipment.StatusEnum)reader.GetInt32(2),
                kit: reader.GetString(3)
            );
        }
        return null;
    }
    public List<Equipment> GetEquipments()
    {
        var equipments = new List<Equipment>();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, kind, status, kit FROM equipments";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            equipments.Add(new Equipment
            (
                id: reader.GetInt64(0),
                kind: (Equipment.KindEnum)reader.GetInt32(1),
                status: (Equipment.StatusEnum)reader.GetInt32(2),
                kit: reader.GetString(3)
            ));
        }
        return equipments;
    }

    public void AddTransaction(Transaction transaction)
    {
        using var dbTransaction = connection.BeginTransaction();
        using var command = connection.CreateCommand();
        command.Transaction = dbTransaction;
        command.CommandText = "UPDATE equipments SET status = @value WHERE id = @id";
        AddParameter(command, "@id", transaction.Equipment.Id);
        AddParameter(command, "@value", (int)transaction.Equipment.Status);
        command.ExecuteNonQuery();
        command.Parameters.Clear();
        command.CommandText = "INSERT INTO transactions (created_at, id_equipment, kind, from_employer, to_employer, note) VALUES (@time, @id, @kind, @employer1, @employer2, @note)";
        AddParameter(command, "@time", transaction.Timestamp);
        AddParameter(command, "@id", transaction.IdEquipment);
        AddParameter(command, "@kind", (int)transaction.Kind);
        AddParameter(command, "@employer1", transaction.IdEmployerFrom);
        AddParameter(command, "@employer2", transaction.IdEmployerTo);
        AddParameter(command, "@note", transaction.Note);
        command.ExecuteNonQuery();
        dbTransaction.Commit();
    }

    public Transaction? GetTransaction(long id)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT t.created_at, t.id_equipment, t.kind, t.from_employer, t.to_employer, e.kind, e.status, t.note, e.kit FROM transactions AS t LEFT JOIN equipments AS e ON e.id = t.id_equipment WHERE t.id_equipment = @id ORDER BY t.created_at DESC LIMIT 1";
        AddParameter(command, "@id", id);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Transaction
                (
                    timestamp: reader.GetDateTime(0),
                    idEquipment: reader.GetInt64(1),
                    kind: (Transaction.KindEnum)reader.GetInt32(2),
                    idEmployerFrom: reader.GetInt32(3),
                    idEmployerTo: reader.GetInt32(4),
                    equipment: new Equipment
                    (
                        id: reader.GetInt64(1),
                        kind: (Equipment.KindEnum)reader.GetInt32(5),
                        status: (Equipment.StatusEnum)reader.GetInt32(6),
                        kit: reader.GetString(8)
                    ),
                    note: reader.IsDBNull(7) ? String.Empty : reader.GetString(7)
                );
        }
        return null;
    }

    public List<Transaction> GetTransactions()
    {
        var transactions = new List<Transaction>();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT t.created_at, e.id, t.kind, t.from_employer, t.to_employer, e.kind, e.status, t.note, e.kit FROM equipments AS e LEFT JOIN transactions AS t ON e.id = t.id_equipment AND t.created_at = (SELECT MAX(tr.created_at) FROM transactions AS tr WHERE tr.id_equipment = e.id)";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            transactions.Add(new Transaction
                (
                    timestamp: reader.IsDBNull(0) ? DateTime.MinValue : reader.GetDateTime(0),
                    idEquipment: reader.IsDBNull(1) ? 0 : reader.GetInt64(1),
                    kind: reader.IsDBNull(2) ? Transaction.KindEnum.Idle : (Transaction.KindEnum)reader.GetInt32(2),
                    idEmployerFrom: reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                    idEmployerTo: reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                    equipment: new Equipment
                    (
                        id: reader.GetInt64(1),
                        kind: (Equipment.KindEnum)reader.GetInt32(5),
                        status: (Equipment.StatusEnum)reader.GetInt32(6),
                        kit: reader.GetString(8)
                    ),
                    note: reader.IsDBNull(7) ? String.Empty : reader.GetString(7)
                )
            );
        }
        return transactions;
    }
    public List<Report> GetGrouping(string group)
    {
        var reports = new List<Report>();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT t.created_at, e.id, t.kind, t.from_employer, t.to_employer, e.kind, e.status, t.note, e.kit FROM equipments AS e LEFT JOIN transactions AS t ON e.id = t.id_equipment AND t.created_at = (SELECT MAX(tr.created_at) FROM transactions AS tr WHERE tr.id_equipment = e.id)";
        AddParameter(command, "@value", group);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            reports.Add(new Report
            {
                    CreateAt: reader.IsDBNull(0) ? DateTime.MinValue : reader.GetDateTime(0),
                    idEquipment: reader.IsDBNull(1) ? 0 : reader.GetInt64(1),
                    kind: reader.IsDBNull(2) ? Transaction.KindEnum.Idle : (Transaction.KindEnum)reader.GetInt32(2),
                    idEmployerFrom: reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                    idEmployerTo: reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                    equipment: new Equipment
                    (
                        id: reader.GetInt64(1),
                        kind: (Equipment.KindEnum)reader.GetInt32(5),
                        status: (Equipment.StatusEnum)reader.GetInt32(6),
                        kit: reader.GetString(8)
                    ),
                    note: reader.IsDBNull(7) ? String.Empty : reader.GetString(7)
                )
            };
        }
        return transactions;
    }
}
