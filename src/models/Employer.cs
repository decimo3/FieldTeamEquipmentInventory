namespace FieldTeamEquipmentInventory.Models;

public class Employer
{
    public int Registry { get; set; }
    public string FullName { get; set; }
    public string Template { get; set; }
    public DateTime CreateAt { get; set; }
    public Employer(int registry, string fullname, string template, DateTime createAt)
    {
        Registry = registry;
        FullName = fullname;
        Template = template;
        CreateAt = createAt;
    }
}