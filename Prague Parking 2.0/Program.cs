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


public abstract class Fordon
{
    public string Fordonstyp { get; set; }
    public string RegNr { get; set; }
    public abstract int Storlek { get; }
    public DateTime AnkomstTid { get; set; } = DateTime.UtcNow;
    public Fordon(string fordonstyp, string regNr)
    {
        if (string.IsNullOrWhiteSpace(regNr) || regNr.Length > 10)
        {
            throw new ArgumentException("Ogiltig inmatning. Registeringsnumret får ej vara tomt eller innehålla fler än 10 tecken. Försök igen.");
        }
        Fordonstyp = fordonstyp;
        RegNr = regNr;
    }

}

public class Bil : Fordon
{
    public Bil(string regNr) : base("Bil", regNr)
    {

    }
    public override int Storlek => 4;
}
public class MC : Fordon
{
    public MC(string regNr) : base("MC", regNr)
    {

    }
    public override int Storlek => 2;
}
public class Buss : Fordon  //Endast påbörjade ej prio
{
    public Buss(string regNr) : base("Buss", regNr)
    {

    }

    public override int Storlek => 16;
}
public class Cykel : Fordon // Endast påbörjade ej prio
{
    public Cykel(string regNr) : base("Cykel", regNr)
    {

    }

    public override int Storlek => 1;
}

public class ParkeringsGarage   //ska allt här verkligen vara publikt ?? - tar bort set - Listan ska ej kunna ändras utifrån
{
    public List<ParkeringsRuta> Rutor { get; } = new List<ParkeringsRuta>();

    private readonly AppConfig _config;
    public int KolumnPerRad => _config.GarageSettings.KolumnerPerRadUtskrift;
    public ParkeringsGarage(AppConfig config)
    {
        _config = config;

        int antalPlatser = _config.GarageSettings.MaxAntalGaragePlatser;
        int maxKapacitetRuta = _config.GarageSettings.MaxKapacitetPerRuta;

        for (int i = 1; i <= _config.GarageSettings.MaxAntalGaragePlatser; i++)
        {
            Rutor.Add(new ParkeringsRuta(i, maxKapacitetRuta)); // Lägger till platserna i garaget och maxkapaciteten per ruta
        }
    }

    public Dictionary<string, int> BeräknaAntalFordon()
    {
        var totalen = new Dictionary<string, int>();
        foreach (var ruta in Rutor)
        {
            foreach (var fordon in ruta.ParkeradeFordon)
            {
                if (totalen.ContainsKey(fordon.Fordonstyp))
                {
                    totalen[fordon.Fordonstyp]++;
                }
                else
                {
                    totalen[fordon.Fordonstyp] = 1;
                }
            }
        }
        return totalen;
    }

    public ParkeringsRuta HittaLedigPlats(Fordon fordon, int valdPlats = 0)
    {
        if (valdPlats > 0)  //Vid önskan om specifik plats
        {
            ParkeringsRuta valdRuta = Rutor[valdPlats - 1]; //från språkval till rätt index.

            if (valdRuta.FinnsLedigPlats(fordon))
            {
                return valdRuta;
            }
            return null;    //Ingen tillgänglig plats på önskad ruta
        }

        else // Letar första bästa plats om ej önskemål angivet
        {
            foreach (var ruta in Rutor)
            {
                if (ruta.FinnsLedigPlats(fordon))
                {
                    return ruta;
                }
            }
        }
        return null;
    }
    public ParkeringsRuta SökaFordonsPlats(string regNr)
    {
        foreach (var ruta in Rutor)
        {
            Fordon letarFordon = ruta.HittaFordonIRuta(regNr);

            if (letarFordon != null)
            {
                return ruta;
            }
        }
        return null;
    }
    public Fordon TaBortFordon(string regNr)
    {
        ParkeringsRuta ruta = SökaFordonsPlats(regNr);
        if (ruta == null)
        {
            return null;
        }
        Fordon fordonAttCheckaUt = ruta.HittaFordonIRuta(regNr);

        if (fordonAttCheckaUt != null)
        {
            ruta.TaBortFordon(regNr);
            return fordonAttCheckaUt;
        }
        return null;
    }

    public Fordon FlyttaFordon(string regNr, int valdPlats)   //Se över denna - allt relevant att kolla ?? {ska man ha try catch istället?
    {
        ParkeringsRuta startRuta = SökaFordonsPlats(regNr);
        if (startRuta == null)  //Om fordonet inte finns - returneras null och fångas upp här
        {
            return null;
        }

        Fordon fordon = startRuta.HittaFordonIRuta(regNr);

        if (startRuta.Plats == valdPlats) //Koll om man försöker flytta till samma plats igen
        {
            return null;
        }

        ParkeringsRuta önskadRuta = HittaLedigPlats(fordon, valdPlats);
        if (önskadRuta == null)
        {
            return fordon;
        }

        önskadRuta.LäggTillFordon(fordon);
        startRuta.TaBortFordon(regNr);

        return fordon;
    }

}

public class ParkeringsRuta
{
    public int Plats { get; }
    public int MaxKapacitet { get; }
    private readonly List<Fordon> _parkeradeFordon = new List<Fordon>();    //Listan ligger privat och ej för vem som helst att ändra
    public IReadOnlyList<Fordon> ParkeradeFordon => _parkeradeFordon.AsReadOnly(); //Ger tillgång utåt att kunna sammanställa listan, se index m.m.
    public ParkeringsRuta(int plats, int maxKapacitetRuta)
    {
        Plats = plats;
        MaxKapacitet = maxKapacitetRuta;
    }

    public bool FinnsLedigPlats(Fordon fordon)
    {
        int befintligtUtrymme = _parkeradeFordon.Sum(f => f.Storlek);
        return (MaxKapacitet - befintligtUtrymme) >= fordon.Storlek;
    }

    public bool LäggTillFordon(Fordon fordon)
    {
        if (!FinnsLedigPlats(fordon))
        {
            return false;
        }
        _parkeradeFordon.Add(fordon);
        return true;
    }
    public bool TaBortFordon(string regNr)
    {
        Fordon fordon = HittaFordonIRuta(regNr);
        if (fordon != null)
        {
            _parkeradeFordon.Remove(fordon);
            return true;
        }
        return false;
    }
    public Fordon HittaFordonIRuta(string regNr)
    {
        foreach (var fordon in ParkeradeFordon)
        {
            if (fordon.RegNr == regNr)
            {
                return fordon;
            }
        }
        return null;
    }


}