

namespace Prague_Parking_Library;

public abstract class Fordon
{
    public string Fordonstyp { get; set; }
    public string RegNr { get; set; }
    public int Storlek { get; set; }
    public DateTime AnkomstTid { get; set; } = DateTime.UtcNow;
    public Fordon(string fordonstyp, string regNr, int storlek)
    {
        if (string.IsNullOrWhiteSpace(regNr) || regNr.Length > 10)
        {
            throw new ArgumentException("Ogiltig inmatning. Registeringsnumret får ej vara tomt eller innehålla fler än 10 tecken. Försök igen.");
        }
        Fordonstyp = fordonstyp;
        RegNr = regNr;
        Storlek = storlek;
    }

}
