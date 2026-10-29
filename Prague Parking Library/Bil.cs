
namespace Prague_Parking_Library;

public class Bil : Fordon
{
    public Bil(string regNr) : base("Bil", regNr)
    {

    }
    public override int Storlek => 4;
}
