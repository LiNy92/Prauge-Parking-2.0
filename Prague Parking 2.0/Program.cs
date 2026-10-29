using Prague_Parking_Library;
using Spectre.Console;

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


public class Meny
{
    public void StartaMeny(AppConfig config)
    {
        var garage = new ParkeringsGarage(config);
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
                        parkeringsvakt.PåbörjaIncheckning(garage, config);
                        break;
                    }
                case "Checka ut fordon":
                    {
                        var parkeringsvakt = new ParkeringsVakt();
                        parkeringsvakt.VerkställUtcheckning(garage, config);
                        break;
                    }
                case "Söka fordon":
                    {
                        var parkeringsvakt = new ParkeringsVakt();
                        string regNr = parkeringsvakt.AngeRegNr();
                        parkeringsvakt.SökaFordonOchPlats(garage);

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




public class PrisLista
{
    private readonly AppConfig _config;
    public PrisLista(AppConfig config)
    {
        _config = config;
    }
    public decimal BeräknaKostnad(Fordon fordon)
    {
        TimeSpan tidParkerad = DateTime.UtcNow - fordon.AnkomstTid;
        decimal antalTimmar = (decimal)Math.Ceiling(tidParkerad.TotalHours);
        if (tidParkerad.TotalMinutes <= _config.FriaMinuter)
        {
            return 0.0M;
        }

        decimal totalKostnad = _config.Priser[fordon.Fordonstyp] * antalTimmar;
        return totalKostnad;
    }
}

public class ParkeringsVakt     //Osäker kring denna klass .. Genomföraren.
{
    public void PåbörjaIncheckning(ParkeringsGarage garage, AppConfig config)
    {
        var fordonstyp = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Vilket fordon önskar du checka in?")
            .AddChoices("Bil", "MC", "Återgå till meny"));

        if (fordonstyp == "Återgå till meny") return;

        string regNr = AngeRegNr();
        if (garage.SökaFordonsPlats(regNr) != null)
        {
            AnsiConsole.MarkupLine($"[red]Ett fordon med registreringsrummer {regNr} finns redan incheckat i garaget![/]");
            return;
        }

        if (AnsiConsole.Confirm("Vill du checka in angivet fordon."))
        {
            try
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
            catch (ArgumentException ex)
            {
                AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
                AnsiConsole.Prompt(
           new TextPrompt<string>("[orange1]Tryck[/] [green]Enter[/] [orange1]för att återgå till meny.[/]")
           .AllowEmpty());
            }
        }
        return;
    }

    public void VerkställUtcheckning(ParkeringsGarage garage, AppConfig config)
    {
        string regNr = AngeRegNr();
        ParkeringsRuta fordonsPlats = garage.SökaFordonsPlats(regNr);
        if (fordonsPlats != null)
        {
            Fordon fordon = fordonsPlats.HittaFordonIRuta(regNr);
            PrisLista prisLista = new PrisLista(config);
            decimal kostnad = prisLista.BeräknaKostnad(fordon);
            TimeSpan tidParkerad = DateTime.UtcNow - fordon.AnkomstTid;
            garage.TaBortFordon(regNr);


            AnsiConsole.MarkupLine("[yellow]=== Utcheckning ===[/]");
            AnsiConsole.MarkupLine($"[green]Fordon: {fordon.Fordonstyp}\n" +
                $"RegNr: {regNr}\n" +
                $"Plats:{fordonsPlats.Plats}\n" +
                $"Parkerad tid: {tidParkerad.Hours} timmar, {tidParkerad.Minutes} minuter\n" +
                $"Att betala:{kostnad:C2}[/]");
            AnsiConsole.MarkupLine("Utcheckning genomförd");
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
    public void SökaFordonOchPlats(ParkeringsGarage garage)
    {
        string regNr = AngeRegNr();
        ParkeringsRuta fordonPlats = garage.SökaFordonsPlats(regNr);
        if (fordonPlats != null)
        {
            Fordon funnetFordon = fordonPlats.HittaFordonIRuta(regNr);
            AnsiConsole.MarkupLine($"[green]Fordon: {funnetFordon.Fordonstyp}\nRegNr: {regNr}\nPlats:{fordonPlats.Plats}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Fordonet hittades inte.[/]");
        }
    }

    public void FlyttaFordon(ParkeringsGarage garage)
    {
        string regNr = AngeRegNr();
        int antalPlatser = garage.Rutor.Count;
        int valdPlats = AnsiConsole.Prompt(new TextPrompt<int>("Vilken plats vill du flytta fordonet till (1-100)?")
            .Validate(plats =>
            {
                if (plats >= 1 && plats <= antalPlatser)
                {
                    return ValidationResult.Success();
                }
                return ValidationResult.Error("[red]Ogiltigt! Du måste välja en plats mellan 1 och 100.[/]");
            }));
        Fordon flyttatFordon = garage.FlyttaFordon(regNr, valdPlats);

        if (flyttatFordon != null)
        {
            AnsiConsole.MarkupLine($"[green]{flyttatFordon.Fordonstyp} {regNr} har flyttats tills plats {valdPlats}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Fordonet kunde ej flyttas till önskad plats.[/]");
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

        int kolumnPerRad = garage.KolumnPerRad;

        for (int i = 1; i <= kolumnPerRad; i++)
        {
            tabell.AddColumn(new TableColumn("").Centered());

        }
        for (int i = 0; i < garage.Rutor.Count; i += kolumnPerRad)
        {
            var rad = new Spectre.Console.Rendering.IRenderable[kolumnPerRad];

            for (int j = 0; j < kolumnPerRad; j++)
            {
                int index = i + j;
                if (index < garage.Rutor.Count)
                {
                    var ruta = garage.Rutor[index];
                    string textIRutan = $"[bold]{ruta.Plats}[/]\n";

                    if (ruta.ParkeradeFordon.Count == 0)
                    {
                        textIRutan += "[green]Ledig[/]";
                    }
                    else
                    {
                        int upptaget = ruta.ParkeradeFordon.Sum(fordon => fordon.Storlek);
                        string färg;
                        if (upptaget == ruta.MaxKapacitet) { färg = "red"; }
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
            ledigPlats.LäggTillFordon(nyttFordon);
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