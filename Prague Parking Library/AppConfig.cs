
namespace Prague_Parking_Library;

public class AppConfig
{
    public GarageConfig GarageSettings { get; set; } = new GarageConfig();
    public Dictionary<string, int> Priser { get; set; } = new Dictionary<string, int>
    {
        ["Bil"] = 20,
        ["MC"] = 10
    };
    public int FriaMinuter { get; set; } = 10;
    public Dictionary<string, int> Storlek { get; set; } = new Dictionary<string, int>
    {
        ["Bil"] = 4,
        ["MC"] = 2
    };
    public List<string> Fordonstyp { get; set; } = new List<string> { "Bil", "MC" };
}