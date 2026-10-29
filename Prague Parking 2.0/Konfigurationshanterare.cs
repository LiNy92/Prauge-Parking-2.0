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
            return new AppConfig();
        }
        string jsonString = File.ReadAllText(filNamn);
        return JsonSerializer.Deserialize<AppConfig>(jsonString) ?? new AppConfig();
    }
}
