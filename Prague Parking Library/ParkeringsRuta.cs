using System.Text.Json.Serialization;

namespace Prague_Parking_Library;


public class ParkeringsRuta
{
    public int Plats { get; }
    public int MaxKapacitet { get; }
    private readonly List<Fordon> _parkeradeFordon = new List<Fordon>();//Listan ligger privat och ej för vem som helst att ändra

    [JsonPropertyName("ParkeradeFordon")]
    public IReadOnlyList<Fordon> ParkeradeFordon => _parkeradeFordon.AsReadOnly(); //Ger tillgång utåt att kunna sammanställa listan, se index m.m.
    public ParkeringsRuta()
    {

    }
    [JsonConstructor]
    public ParkeringsRuta(int plats, int maxKapacitet, List<Fordon> parkeradeFordon)
    {
        Plats = plats;
        MaxKapacitet = maxKapacitet;
        if (parkeradeFordon != null)
        {
            _parkeradeFordon = parkeradeFordon;
        }
    }
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
