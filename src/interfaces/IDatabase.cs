using FieldTeamEquipmentInventory.Models;

namespace FieldTeamEquipmentInventory.Interfaces;

public interface IDatabase
{
    public void AddEmployer(Employer employer);
    public Employer? GetEmployer(int id);
    public List<Employer> GetEmployers();
    public void AddEquipment(Equipment equipment);
    public Equipment? GetEquipment(long id);
    public List<Equipment> GetEquipments();
    public void AddTransaction(Transaction transaction);
    public Transaction? GetTransaction(long id);
    public List<Transaction> GetTransactions();
}
