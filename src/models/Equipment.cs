namespace FieldTeamEquipmentInventory.Models;

public class Equipment
{
    public long Id { get; set; }
    public KindEnum Kind { get; set; }
    public StatusEnum Status { get; set; }
    public string Kit { get; set; }
    public enum KindEnum { Aferidor = 1, Amperimetro = 2, Maquineta = 3, Sequencimetro = 4, Smartphone = 5 }
    public enum StatusEnum { Wrecked = -1, Ok = 0, Damaged = 1 }
    public Equipment(long id, KindEnum kind, StatusEnum status, string kit)
    {
        Id = id;
        Kind = kind;
        Status = status;
        Kit = kit;
    }
}