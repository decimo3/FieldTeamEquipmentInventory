using Serilog;
using Serilog.Events;

namespace FieldTeamEquipmentInventory.Services;

public static class Logger
{
    private static ILogger? _logger;

    public static void Configure(string? outputDirectory = null, LogEventLevel level = LogEventLevel.Information)
    {
        if (_logger != null) return;

        outputDirectory ??= System.AppContext.BaseDirectory;
        var logPath = System.IO.Path.Combine(outputDirectory, "logs", "app-.log");

        _logger = new LoggerConfiguration()
            .MinimumLevel.Is(level)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            )
            .CreateLogger();

        Serilog.Log.Logger = _logger;
    }

    public static ILogger Instance
    {
        get
        {
            if (_logger == null)
                Configure();
            return Serilog.Log.Logger;
        }
    }

    public static void Shutdown()
    {
        Serilog.Log.CloseAndFlush();
        _logger = null;
    }
}
