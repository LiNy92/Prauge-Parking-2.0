namespace Prague_Parking_Library;

public class PrisLista
{
    private readonly AppConfig _config;
    public PrisLista(AppConfig config)
    {
        _config = config;
    }
    public decimal BeräknaKostnad(Fordon fordon)
    {
        TimeSpan tidParkerad = DateTime.UtcNow - fordon.AnkomstTid;
        decimal antalTimmar = (decimal)Math.Ceiling(tidParkerad.TotalHours);
        if (tidParkerad.TotalMinutes <= _config.FriaMinuter)
        {
            return 0.0M;
        }

        decimal totalKostnad = _config.Priser[fordon.Fordonstyp] * antalTimmar;
        return totalKostnad;
    }
}
