
namespace Prague_Parking_Library;

public class AppConfig
{
    public GarageConfig GarageSettings { get; set; } = new GarageConfig();
    public int FriaMinuter { get; set; } = 10;
    public List<string> Fordonstyp { get; set; } = new List<string>();
    public Dictionary<string, int> Priser {  get; set; } = new Dictionary<string, int>(); 
}
