using Prague_Parking_2._0;
using Spectre.Console;

namespace Prague_Parking_2._0;

internal class Program
{
    static void Main(string[] args)
    {
        var meny = new Meny();
        meny.StartaMeny();

    }

}







public class Meny
{
    public void StartaMeny()
    {
        var garage = new ParkeringsGarage();
        while (true)
        {

            var menyVal = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Meny - Prauge Parking")
                    .AddChoices("Checka in fordon", "Checka ut fordon", "Söka fordon", "Flytta fordon", "Visa prislista", "Visa parkeringsgarage", "Avsluta program"));

            AnsiConsole.MarkupLine($"Du har valt: [green]{menyVal}[/]");
            switch (menyVal)
            {
                case "Checka in fordon":
                    {
                        var parkeringsvakt = new ParkeringsVakt();
                        parkeringsvakt.PåbörjaIncheckning(garage);
                        break;
                    }
                case "Checka ut fordon":
                    {
                        var parkeringsvakt = new ParkeringsVakt();
                        parkeringsvakt.GenomförUtcheckning(garage);
                        break;
                    }
                case "Söka fordon":
                    {
                        var parkeringsvakt = new ParkeringsVakt();
                        string regNr = parkeringsvakt.AngeRegNr();
                        parkeringsvakt.SökaFordon(garage);

                        break;
                    }
                case "Flytta fordon":
                    {
                        break;
                    }
                case "Visa prislista":
                    { break; }
                case "Visa parkeringsgarage":
                    { break; }
                case "Avsluta program":
                    { return; }
            }
        }
    }
}

public class ParkeringsVakt
{
    public void PåbörjaIncheckning(ParkeringsGarage garage)      //Kolla över detta namn , garagevariabeln .
    {
        var fordonstyp = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Vilket fordon önskar du checka in?")
            .AddChoices("Bil", "MC", "Återgå till meny"));

        if (fordonstyp == "Återgå till meny") return;

        string regNr = AngeRegNr();
        if (AnsiConsole.Confirm("Vill du checka in angivet fordon."))
        {
            Fordon nyttFordon = null;
            switch (fordonstyp)
            {
                case "Bil":
                    nyttFordon = new Bil(regNr);
                    break;
                case "MC":
                    nyttFordon = new MC(regNr);         //Add till listan
                    break;
            }
            if (nyttFordon != null)
            {
                ParkeraFordon(garage, nyttFordon, regNr);
            }
        }
        return;
    }

    public void GenomförUtcheckning(ParkeringsGarage garage)
    {
        string regNr = AngeRegNr();
        ParkeringsRuta funnenRuta = garage.SökaFordon(regNr);
        if (funnenRuta != null)
        {
            Fordon funnetFordon = funnenRuta.HittaFordonIRuta(regNr);
            garage.TaBortFordon(regNr);
            AnsiConsole.MarkupLine("Utcheckning:");
            AnsiConsole.MarkupLine($"[green]Fordon: {funnetFordon.Fordonstyp}\nRegNr: {regNr}\nPlats:{funnenRuta.Plats}[/]");
            AnsiConsole.MarkupLine("Genomförd");
        }
        else
        {
            AnsiConsole.MarkupLine("Det gick ej att utföra utcheckningen.");
        }
    }

    public string AngeRegNr()
    {
        var regNr = AnsiConsole.Ask<string>("[green]\nVänligen ange registreringsnummer (max 10 tecken):[/]").ToUpper().Trim();
        AnsiConsole.MarkupLine($"Du angav registreringsnummer: [red]{regNr}[/]");
        return regNr;
    }
    public void SökaFordon(ParkeringsGarage garage)
    {
        string regNr = AngeRegNr();
        ParkeringsRuta funnenRuta = garage.SökaFordon(regNr);
        if (funnenRuta != null)
        {
            Fordon funnetFordon = funnenRuta.HittaFordonIRuta(regNr);
            AnsiConsole.MarkupLine($"[green]Fordon: {funnetFordon.Fordonstyp}\nRegNr: {regNr}\nPlats:{funnenRuta.Plats}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Fordonet hittades inte.[/]");
        }
    }

    public void VisaParkeringsGarage(ParkeringsGarage garage)
    {

    }
    private void ParkeraFordon(ParkeringsGarage garage, Fordon nyttFordon, string regNr)
    {
        ParkeringsRuta ledigPlats = garage.HittaLedigPlats(nyttFordon);
        if (ledigPlats != null)
        {
            ledigPlats.ParkeradeFordon.Add(nyttFordon);
            AnsiConsole.MarkupLine($"[green]Fordon: {nyttFordon.Fordonstyp}\nRegistreringnummer: {regNr}\nIncheckad på plats: {ledigPlats.Plats}  [/]");
        }
        else
        {

            AnsiConsole.MarkupLine($"[red]Ingen ledig plats hittades.[/]");
        }
    }
}


public abstract class Fordon
{
    public string Fordonstyp { get; set; }
    public string RegNr { get; set; }
    public int Storlek
    {
        get
        {
            if (Fordonstyp == "MC")
            {
                return 2;
            }
            return 4;       //Returnera 4 vid bil.
        }
    }
    public DateTime AnkomstTid { get; set; } = DateTime.Now;
    public DateTime? UtcheckningsTid { get; set; } = null;

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
}
public class MC : Fordon
{
    public MC(string regNr) : base("MC", regNr)
    {

    }
}
public class Buss : Fordon  //Endast påbörjade ej prio
{
    public Buss(string regNr) : base("Buss", regNr)
    {

    }
}
public class Cykel : Fordon // Endast påbörjade ej prio
{
    public Cykel(string regNr) : base("Cykel", regNr)
    {

    }
}

public class ParkeringsGarage
{
    public List<ParkeringsRuta> Rutor { get; set; } = new List<ParkeringsRuta>();

    public ParkeringsGarage()
    {
        for (int i = 1; i <= 100; i++)
        {
            Rutor.Add(new ParkeringsRuta(i)); // Lägger till plats 1-100 i garaget
        }
    }

    public ParkeringsRuta HittaLedigPlats(Fordon fordon)
    {
        foreach (var ruta in Rutor)
        {
            int upptagetUtrymme = ruta.ParkeradeFordon.Sum(fordon => fordon.Storlek);
            int ledigtUtrymme = 4 - upptagetUtrymme;
            if (ledigtUtrymme >= fordon.Storlek)
            {
                return ruta;
            }
        }
        return null;
    }
    public ParkeringsRuta SökaFordon(string regNr)
    {
        foreach (var ruta in Rutor)
        {
            Fordon funnetFordon = ruta.HittaFordonIRuta(regNr);

            if (funnetFordon != null)
            {
                return ruta;
            }
        }
        return null;
    }
    public Fordon TaBortFordon(string regNr)
    {
        ParkeringsRuta ruta = SökaFordon(regNr);
        if (ruta == null)
        {
            return null;
        }
        Fordon fordonAttCheckaUt = ruta.HittaFordonIRuta(regNr);

        if (fordonAttCheckaUt != null)
        {
            ruta.ParkeradeFordon.Remove(fordonAttCheckaUt);
            return fordonAttCheckaUt;
        }
        return null;
    }


}

public class ParkeringsRuta
{
    public int Plats { get; }
    public List<Fordon> ParkeradeFordon { get; set; } = new List<Fordon>();
    public ParkeringsRuta(int plats)
    {
        Plats = plats;
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