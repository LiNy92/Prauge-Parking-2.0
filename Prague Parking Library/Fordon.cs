

namespace Prague_Parking_Library;

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
