namespace Prague_Parking_Library;

public class GarageConfig
{
    public int MaxAntalGaragePlatser { get; set; } = 100;   //Ger startvärden ifall filen av någon anledning ej går att läsa.
    public int MaxKapacitetPerRuta { get; set; } = 4;
    public int KolumnerPerRadUtskrift { get; set; } = 10;
}
