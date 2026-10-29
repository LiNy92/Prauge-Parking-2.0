
namespace Prague_Parking_Library
{
    public class AppConfig
    {
        public GarageConfig GarageSettings { get; set; }
        public int FriaMinuter { get; set; } = 10;
        public Dictionary<string, int> Priser {  get; set; } = new Dictionary<string, int>(); 
    }
}
