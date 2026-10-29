using Prague_Parking_Library;

namespace Prague_Parking_2._0;

internal class Program
{
    static void Main(string[] args)
    {
        AppConfig config = Konfigurationshanterare.Ladda();

        var meny = new Meny();
        meny.StartaMeny(config);

    }

}