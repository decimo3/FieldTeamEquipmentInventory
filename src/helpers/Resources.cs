namespace FieldTeamEquipmentInventory.Helpers;

public static class Resources
{
    private readonly static Dictionary<string, string> _VALUES;

    static Resources()
    {
        var cultureInfo = System.Globalization.CultureInfo.CurrentCulture;
        var resourcesFilename = $"strings_{cultureInfo.Name}.res";
        var resourcesFilepath = System.IO.Path.Combine(
            System.AppContext.BaseDirectory, "assets", resourcesFilename);
        _VALUES = ReadFile(resourcesFilepath);
    }

    private static Dictionary<String, String> ReadFile(String filename, char delimiter = '=')
    {
        var configurations = new Dictionary<string, string>();
        var file = System.IO.File.ReadAllLines(filename);
        foreach (var line in file)
        {
            if (String.IsNullOrEmpty(line)) continue;
            if (line.TrimStart().StartsWith('#')) continue;
            var args = line.Split(delimiter, 2);
            if (args.Length < 2) continue;
            var key = args[0].Trim();
            var val = args[1].Trim();
            configurations[key] = val;
        }
        return configurations;
    }

    public static string GetString(string placeholder, params object?[] args)
    {
        if (String.IsNullOrWhiteSpace(placeholder))
            throw new ArgumentException(_VALUES["RESOURCE_NOT_FOUND"]);

        if (!_VALUES.TryGetValue(placeholder, out string? value) || value is null)
            throw new ArgumentException(String.Format(_VALUES["RESOURCE_NOT_FOUND"], placeholder));

        return args is null ? value : String.Format(value, args).Replace("\\n", "\n");
    }
}