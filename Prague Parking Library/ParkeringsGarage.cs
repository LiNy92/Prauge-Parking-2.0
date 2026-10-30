
namespace Prague_Parking_Library;

public class ParkeringsGarage
{
    private readonly List<ParkeringsRuta> _rutor = new List<ParkeringsRuta>();
    public IReadOnlyList<ParkeringsRuta> Rutor => _rutor.AsReadOnly();
    public int KolumnPerRad { get; }
    public ParkeringsGarage(AppConfig config)
    {
        KolumnPerRad = config.GarageSettings.KolumnerPerRadUtskrift;
        int antalPlatser = config.GarageSettings.MaxAntalGaragePlatser;
        int maxKapacitetRuta = config.GarageSettings.MaxKapacitetPerRuta;

        for (int i = 1; i <= antalPlatser; i++)
        {
            _rutor.Add(new ParkeringsRuta(i, maxKapacitetRuta));
        }
    }

    public bool LäggTillFordon(Fordon fordon, int valdPlats)
    {
        if (valdPlats < 1 || valdPlats > _rutor.Count || SökaFordonsPlats(fordon.RegNr) != null)
        {
            return false;
        }

        return _rutor[valdPlats - 1].LäggTillFordon(fordon);
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
        if (valdPlats < 0 || valdPlats > _rutor.Count)
        {
            return null;
        }

        if (valdPlats > 0)
        {
            ParkeringsRuta valdRuta = _rutor[valdPlats - 1];
            if (valdRuta.SökLedigPlats(fordon))
            {
                return valdRuta;
            }
            return null;
        }

        foreach (var ruta in Rutor)
        {
            if (ruta.SökLedigPlats(fordon))
            {
                return ruta;
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

    public Flyttresultat FlyttaFordon(string regNr, int valdPlats)
    {
        if (valdPlats < 1 || valdPlats > _rutor.Count)
        {
            return Flyttresultat.OgiltigPlats;
        }

        ParkeringsRuta startRuta = SökaFordonsPlats(regNr);
        if (startRuta == null)
        {
            return Flyttresultat.FordonSaknas;
        }

        Fordon fordon = startRuta.HittaFordonIRuta(regNr);
        if (startRuta.Plats == valdPlats)
        {
            return Flyttresultat.SammaPlats;
        }

        ParkeringsRuta önskadRuta = HittaLedigPlats(fordon, valdPlats);
        if (önskadRuta == null || !önskadRuta.LäggTillFordon(fordon))
        {
            return Flyttresultat.UtrymmeSaknas;
        }

        startRuta.TaBortFordon(regNr);
        return Flyttresultat.Lyckats;
    }

}
