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

    public static void CanBeTransacted(Transaction? curr, Transaction next)
    {
        // if current transaction is null, treat then as Idle
        if (curr is null)
        {
            if (next.Kind != KindEnum.Checkin)
                throw new InvalidOperationException(Resources.GetString("MODEL_TRANSACTION_IDLE_TO", next.Kind));
            return;
        }
        // If next transactions object is null, reject
        if (next is null)
                throw new ArgumentNullException(Resources.GetString("MODEL_TRANSACTION_NEXT_NULL"));
        // If current transaction state is removed, 
        if (curr.Kind == KindEnum.Removed)
            throw new InvalidOperationException(Resources.GetString("MODEL_TRANSACTION_curr_REMOVED"));
        // If next transaction state is idle, reject
        if (next.Kind == KindEnum.Idle)
            throw new InvalidOperationException(Resources.GetString("MODEL_TRANSACTION_NEXT_IDLE"));
        // If both transactions are the same, reject
        if (curr.Kind == next.Kind)
            throw new InvalidOperationException(Resources.GetString("MODEL_TRANSACTION_BOTH_SAME", next.Kind));
        // If current transaction is idle and next is not check-in, reject
        // Idle status is only able to check-in, don't make sense receive another status
        if (curr.Kind == KindEnum.Idle && next.Kind != KindEnum.Checkin)
            throw new InvalidOperationException(Resources.GetString("MODEL_TRANSACTION_IDLE_TO", next.Kind));
        // If current transaction is checkin and next is not checkout, reject
        // Check-in status is only able to checkout, even to go to repair or remove they must be checkout first!
        if (curr.Kind == KindEnum.Checkin && next.Kind != KindEnum.Checkout)
            throw new InvalidOperationException(Resources.GetString("MODEL_TRANSACTION_CHECKOUT_TO"));
        // If current transaction is ToRepair and next is not FromRepair, reject
        // ToRepair status is only able to FromRepair, don't make sense receive another status
        if (curr.Kind == KindEnum.ToRepair && next.Kind != KindEnum.FromRepair)
            throw new InvalidOperationException(Resources.GetString("MODEL_TRANSACTION_FROMREPAIR_TO", next.Kind));
        if (next.Kind == KindEnum.Checkin && (curr.Kind != KindEnum.Checkout && curr.Kind != KindEnum.Idle))
            throw new InvalidOperationException(Resources.GetString("MODEL_TRANSACTION_CHECKIN_TO"));
        if (next.Kind == KindEnum.ToRepair && curr.Kind != KindEnum.Checkout)
            throw new InvalidOperationException(Resources.GetString("MODEL_TRANSACTION_TOREPAIR_TO"));
        if (next.Kind == KindEnum.Removed && curr.Kind != KindEnum.Checkout)
            throw new InvalidOperationException(Resources.GetString("MODEL_TRANSACTION_REMOVED_TO"));
    }
}
