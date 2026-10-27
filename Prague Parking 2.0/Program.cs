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
                        parkeringsvakt.HittaFordonsplats(garage);

                        break;
                    }
                case "Flytta fordon":
                    {
                        var parkeringsvakt = new ParkeringsVakt();
                        parkeringsvakt.FlyttaFordon(garage);
                        break;
                    }
                case "Visa prislista":
                    { break; }
                case "Visa parkeringsgarage":
                    {
                        var parkeringsvakt = new ParkeringsVakt();
                        parkeringsvakt.VisaParkeringsGarage(garage);

                        break;
                    }
                case "Avsluta program":
                    { return; }
            }
        }
    }
}

public class ParkeringsVakt     //
{
    public void PåbörjaIncheckning(ParkeringsGarage garage)
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
                    nyttFordon = new MC(regNr);
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
    public void HittaFordonsplats(ParkeringsGarage garage)
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

    public void FlyttaFordon(ParkeringsGarage garage)
    {
        string regNr = AngeRegNr();
        int valdPlats = AnsiConsole.Prompt(new TextPrompt<int>("Vilken plats vill du flytta fordonet till (1-100)?")
            .Validate(plats =>
            {
                if (plats >= 1 && plats <= 100)
                {
                    return ValidationResult.Success();
                }
                return ValidationResult.Error("[red]Ogiltigt! Du måste välja en plats mellan 1 och 100.[/]");
            }));
        bool lyckadFlytt = garage.FlyttaFordon(regNr, valdPlats);
        if (lyckadFlytt)
        {
            AnsiConsole.MarkupLine($"Fordon {regNr} har flyttats tills plats {valdPlats}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Fordonet kunde ej flyttas till önskad plats.");
        }
    }
    public void VisaParkeringsGarage(ParkeringsGarage garage)
{
    VisaStatistik(garage);
    VisaParkeringsplatser(garage);
    AnsiConsole.Prompt(
        new TextPrompt<string>("Tryck [green]Enter[/] för att återgå till meny.")
        .AllowEmpty());
}
private void VisaStatistik(ParkeringsGarage garage)
{
    var antalFordon = garage.BeräknaAntalFordon();
    int totaltAntalFordon = antalFordon.Values.Sum();
    int totalaRutor = garage.Rutor.Count;
    //int upptagnaRutor = garage.Rutor.Count(ruta => ruta.ParkeradeFordon.Count > 0);

    string grundText = $"[bold]Antal fordon:[/] {totaltAntalFordon}\n\n [bold]Parkerade fordon:[/]\n";

    if (antalFordon.Count > 0)
    {
        foreach (var fordonstyp in antalFordon)
        {
            grundText += $"{fordonstyp.Key}: {fordonstyp.Value}\n";
        }
    }
    else
    {
        grundText += "Inga fordon incheckade";
    }

    var statistikPanel = new Panel(grundText.TrimEnd())
        .Header("[yellow]Sammanställning[/]")
        .Border(BoxBorder.Rounded)
        .Padding(1, 1);

    AnsiConsole.Write(statistikPanel);
}
private void VisaParkeringsplatser(ParkeringsGarage garage)
{
    var tabell = new Table();
    tabell.Border(TableBorder.None);
    tabell.HideHeaders();
    tabell.Title("[bold cyan]Prague Parking - Garageöversikt[/]");

    for (int i = 1; i <= 10; i++)
    {
        tabell.AddColumn(new TableColumn("").Centered());

    }
    for (int i = 0; i < garage.Rutor.Count; i += 10)
    {
        var rad = new Spectre.Console.Rendering.IRenderable[10];

        for (int j = 0; j < 10; j++)
        {
            int index = i + j;
            if (index < garage.Rutor.Count)
            {
                var ruta = garage.Rutor[index];
                string textIRutan = $"[bold]Plats {ruta.Plats}[/]\n";

                if (ruta.ParkeradeFordon.Count == 0)
                {
                    textIRutan += "[green]Ledig[/]";
                }
                else
                {
                    int upptaget = ruta.ParkeradeFordon.Sum(fordon => fordon.Storlek);
                    string färg;
                    if (upptaget == 4) { färg = "red"; }
                    else { färg = "orange1"; }

                    foreach (var fordon in ruta.ParkeradeFordon)
                    {
                        textIRutan += $"[{färg}] {fordon.Fordonstyp} {fordon.RegNr}[/]";
                    }

                }
                rad[j] = new Panel(textIRutan)
                     .Border(BoxBorder.Rounded)
                        .PadLeft(0)
                        .PadRight(0);
            }
        }
        tabell.AddRow(rad);
    }
    AnsiConsole.Write(tabell);
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

public class ParkeringsGarage   //ska allt här verkligen vara publikt ??
{
    public List<ParkeringsRuta> Rutor { get; set; } = new List<ParkeringsRuta>();

    public ParkeringsGarage()
    {
        for (int i = 1; i <= 100; i++)
        {
            Rutor.Add(new ParkeringsRuta(i)); // Lägger till plats 1-100 i garaget
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
            ParkeringsRuta valdRuta = null;
            foreach (var ruta in Rutor)
            {
                if (ruta.Plats == valdPlats)
                {
                    valdRuta = ruta;
                    break;
                }
            }
            int platsUtrymme = valdRuta.ParkeradeFordon.Sum(fordon => fordon.Storlek);
            if ((4 - platsUtrymme) >= fordon.Storlek)
            {
                return valdRuta;
            }
            return null;    //Ingen tillgänglig plats på önskad ruta
        }

        else
        {
            foreach (var ruta in Rutor) //Letar första bästa plats om ej önskemål angivet
            {
                int upptagetUtrymme = ruta.ParkeradeFordon.Sum(fordon => fordon.Storlek);
                int ledigtUtrymme = 4 - upptagetUtrymme;
                if (ledigtUtrymme >= fordon.Storlek)
                {
                    return ruta;
                }
            }
        }
        return null;
    }
    public ParkeringsRuta SökaFordon(string regNr)
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

    public bool FlyttaFordon(string regNr, int valdPlats)   //Se över denna - allt relevant att kolla ?? ska man ha try catch istället?
    {
        ParkeringsRuta startRuta = SökaFordon(regNr);
        if (startRuta == null)
        {
            return false;   //Fordonet finne ej i garaget
        }

        Fordon fordon = startRuta.HittaFordonIRuta(regNr);
        if (fordon == null)
        {
            return false;
        }
        if (startRuta.Plats == valdPlats) //Koll om man försöker flytta till samma plats igen
        {
            return true;
        }

        ParkeringsRuta önskadRuta = HittaLedigPlats(fordon, valdPlats);
        if (önskadRuta == null)
        {
            return false;
        }

        startRuta.ParkeradeFordon.Remove(fordon);
        önskadRuta.ParkeradeFordon.Add(fordon);

        return true;
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