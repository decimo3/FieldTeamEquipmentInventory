namespace FieldTeamEquipmentInventory;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        #if DEBUG
            dotenv.net.DotEnv.Load(
                new dotenv.net.DotEnvOptions(
                    envFilePaths: ["dev.env"]));
        #endif
        var app = new MainApplication();
        var window = new MainWindow(); 
        app.Run(window);
    }
}
