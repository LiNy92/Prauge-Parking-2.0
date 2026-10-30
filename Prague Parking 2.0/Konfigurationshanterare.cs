using Prague_Parking_Library;
using System.Text.Json;

namespace Prague_Parking_2._0;

public class Konfigurationshanterare
{
    public static AppConfig Ladda()
    {
        string filNamn = "Config.json";

        if (!File.Exists(filNamn))
        {
            Console.Error.WriteLine("Config.json saknas. Standardinställningar används.");
            return new AppConfig();
        }

        try
        {
            string jsonString = File.ReadAllText(filNamn);
            return JsonSerializer.Deserialize<AppConfig>(jsonString) ?? new AppConfig();
        }
        catch (JsonException)
        {
            Console.Error.WriteLine("Config.json innehåller ogiltig JSON. Standardinställningar används.");
            return new AppConfig();
        }
        catch (IOException)
        {
            Console.Error.WriteLine("Config.json kunde inte läsas. Standardinställningar används.");
            return new AppConfig();
        }
        catch (UnauthorizedAccessException)
        {
            Console.Error.WriteLine("Programmet saknar behörighet att läsa Config.json. Standardinställningar används.");
            return new AppConfig();
        }
    }
}
