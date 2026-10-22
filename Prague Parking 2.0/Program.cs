using Spectre.Console;

namespace Prague_Parking_2._0;

internal class Program
{
    static void Main(string[] args)
    {



        var menyVal = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Meny - Prauge Parking")
                .AddChoices("Checka in fordon", "Checka ut fordon", "Söka fordon", "Flytta fordon", "Visa prislista", "Visa parkeringsgarage", "Avsluta program"));

        AnsiConsole.MarkupLine($"Du har valt: [green]{menyVal}[/]");







    }
}


public class Fordon
{
    public string Fordonstyp { get; set; }
    public string RegNr { get; set; }
    public int Storlek { get; set; }
    public DateTime AnkomstTid { get; set; } = DateTime.Now;
    public DateTime UtcheckningsTid { get; set; } = DateTime.Now;

    public Fordon(string fordonstyp,string regNr, int storlek)
    {
        Fordonstyp = fordonstyp;
        RegNr = regNr;
        Storlek = storlek;
    }
}

public class Bil : Fordon
{
    public Bil(string regNr) : base ("Bil", regNr, 4)
    {
        
    }
}
public class MC : Fordon
{
    public MC(string regNr) : base ("MC", regNr, 2)
    {
        
    }
}
public class Buss : Fordon
{
    public Buss(string regNr) : base("Buss", regNr, 16)
    {
        
    }
}
public class Cykel : Fordon
{
    public Cykel(string regNr) : base("Cykel", regNr, 1)
    {

    }
}

public class ParkeringsGarage
{
    public List<int> ParkeringsPlats { get; set; } = new List<int>();

    public ParkeringsGarage()
    {
        for (int i = 0; i < 100, i++)
        {
            
        }
    }
}


public class ParkeringsRuta : ParkeringsGarage
{
    
}