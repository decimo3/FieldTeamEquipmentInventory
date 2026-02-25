using System.Text.RegularExpressions;

namespace FieldTeamEquipmentInventory.Helpers;

public static class Validators
{
    private readonly static Regex TxtOnly = new("[^A-z ]");
    private readonly static Regex NumOnly = new("[^0-9]");
    public static void NumberInputOnly(object sender, System.Windows.Input.TextCompositionEventArgs e) => e.Handled = NumOnly.IsMatch(e.Text);
    public static void CharacterInputOnly(object sender, System.Windows.Input.TextCompositionEventArgs e) => e.Handled = TxtOnly.IsMatch(e.Text);
}
