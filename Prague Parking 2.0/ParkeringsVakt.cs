using Prague_Parking_Library;
using Spectre.Console;

namespace Prague_Parking_2._0;

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
