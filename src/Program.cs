namespace FieldTeamEquipmentInventory;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        var app = new MainApplication();
        var window = new MainWindow(); 
        app.Run(window);
    }
}
