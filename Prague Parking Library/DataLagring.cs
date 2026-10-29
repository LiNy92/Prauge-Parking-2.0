
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
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(garage.Rutor, Options);
            File.WriteAllText(Filnamn, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Kunde inte spara garaget: {ex.Message}");
        }
    }

    public static List<ParkeringsRuta>? LaddaGarage()
    {
        if(!File.Exists(Filnamn))
        {
            return null;
        }

        try
        {
            string jsonString = File.ReadAllText(Filnamn);
            return JsonSerializer.Deserialize<List<ParkeringsRuta>>(jsonString, Options);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Kunde inte läsa in sparad fil: {ex.Message}");
        }
    }

    }