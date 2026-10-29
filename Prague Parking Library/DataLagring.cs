
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Prague_Parking_Library;

public class DataLagring
{
    private static readonly string Filnamn = "garagedata.json";
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static void SparaGarage(ParkeringsGarage garage)

}