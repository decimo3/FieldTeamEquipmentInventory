namespace FieldTeamEquipmentInventory.Models;

public class Report
{
    public String CreateAt { get; set; }
    public Transaction.KindEnum TransactionKind { get; set; }
    public long IdEquipment { get; set; }
    public Equipment.KindEnum EquipmentKind { get; set; }
    public Equipment.StatusEnum EquipmentStatus { get; set; }
    public string Kit { get; set; }
    public int IdEmployerFrom { get; set; }
    public int IdEmployerTo { get; set; }
    public string? Note { get; set; }

}
