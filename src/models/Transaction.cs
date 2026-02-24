using FieldTeamEquipmentInventory.Helpers;

namespace FieldTeamEquipmentInventory.Models;

public class Transaction
{
    public DateTime Timestamp { get; set; }
    public long IdEquipment { get; set; }
    public KindEnum Kind { get; set; } = KindEnum.Idle;
    public int IdEmployerFrom { get; set; }
    public int IdEmployerTo { get; set; }
    public string? Note { get; set; }
    public Equipment Equipment { get; set; }
    public enum KindEnum { Idle = 0, Checkin = 1, Checkout = 2, ToRepair = 3, FromRepair = 4, Removed = 5 }

    public Transaction
    (
        DateTime timestamp,
        long idEquipment,
        KindEnum kind,
        int idEmployerFrom,
        int idEmployerTo,
        Equipment equipment,
        string? note = null
    )
    {
        Timestamp = timestamp;
        IdEquipment = idEquipment;
        Kind = kind;
        IdEmployerFrom = idEmployerFrom;
        IdEmployerTo = idEmployerTo;
        Equipment = equipment;
        Note = note;
    }
}
