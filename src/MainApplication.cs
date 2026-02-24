using System.Windows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using FieldTeamEquipmentInventory.Interfaces;
using FieldTeamEquipmentInventory.Services;
using FieldTeamEquipmentInventory.Screens;

namespace FieldTeamEquipmentInventory;

public partial class MainApplication : Application
{
    public static IHost? Host { get; private set; }
    public MainApplication()
	{
        Host = Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<MainWindow>();
                services.AddTransient<AuthScreen>();
                services.AddTransient<MainScreen>();
                services.AddTransient<HelpScreen>();
                services.AddTransient<EquipScreen>();
                services.AddTransient<EnrollScreen>();
                services.AddTransient<EntryScreen>();
                services.AddTransient<ReportScreen>();

                // Conditionally register Database implementation based on USE_SQLITE
                // environment variable.
                // If env var is null OR if env var fail to parse OR
                // If env var parsed value is true, then use SQLite
                // Only if all of this checks are false, then use PostgreSQL
                var useSqliteEnv = System.Environment.GetEnvironmentVariable("USE_SQLITE");
                if (string.IsNullOrEmpty(useSqliteEnv) || !bool.TryParse(useSqliteEnv, out bool parsed) || parsed)
                    services.AddSingleton<IDatabase, SQLiteDB>();
                else
                    services.AddSingleton<IDatabase, Postgres>();

                // Conditionally register Biometric implementation based on USE_FINGER
                // environment variable.
                // If env var is null OR if env var fail to parse OR
                // If env var parsed value is true, then use FacialRecognition
                // Only if all of this checks are false, then use Fingerprint
                var useFingerEnv = System.Environment.GetEnvironmentVariable("USE_FINGER");
                if (string.IsNullOrEmpty(useFingerEnv) || !bool.TryParse(useFingerEnv, out parsed) || parsed)
                    services.AddSingleton<IBiometrics, Fingerprint>();
                else
                    services.AddSingleton<IBiometrics, FacialRecognition>();
            })
            .Build();
    }
    protected override async void OnStartup(StartupEventArgs e)
    {
        await Host!.StartAsync();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await Host!.StopAsync();
        Host.Dispose();
        base.OnExit(e);
    }
}

