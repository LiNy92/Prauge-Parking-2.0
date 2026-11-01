using Prague_Parking_Library;
using Spectre.Console;

namespace Prague_Parking_2._0;

public class ParkeringsVakt
{
    public void Incheckning(ParkeringsGarage garage, AppConfig config)
    {
        VisaPrislista();
        var fordonstyp = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Vilket fordon önskar du checka in?")
                .HighlightStyle(new Style(foreground: Color.LightPink3))
                .AddChoices(config.Fordonstyp)
                .AddChoices("Avbryt"));

        if (fordonstyp == "Avbryt") return;

        string regNr = AngeRegNr();

        if (garage.SökaFordonsPlats(regNr) != null)
        {
            AnsiConsole.MarkupLine($"[LightCoral]Ett fordon med registreringsrummer {regNr} finns redan incheckat i garaget![/]");
            return;
        }

        if (AnsiConsole.Confirm("Vill du checka in angivet fordon."))
        {
            Fordon nyttFordon = null;
            if (!config.Storlek.ContainsKey(fordonstyp))
            {
                throw new InvalidOperationException($"Hittade ingen storlek för fordonstypen: {fordonstyp}");
            }
            int storlek = config.Storlek[fordonstyp];

            switch (fordonstyp)
            {
                case "Bil":
                    nyttFordon = new Bil(regNr, storlek);
                    break;
                case "MC":
                    nyttFordon = new MC(regNr, storlek);
                    break;
                case "Buss":    //Förberedda platser för V2.2
                    break;
                case "Cykel":
                    break;
            }
            ParkeringsRuta ledigRuta = garage.HittaLedigPlats(nyttFordon);

            if (ledigRuta == null)
            {
                AnsiConsole.MarkupLine("[LightCoral]Ingen ledig plats hittades.[/]");
                return;
            }
            ledigRuta.LäggTillFordon(nyttFordon);

            AnsiConsole.MarkupLine($"[PaleGreen3_1]Fordon:[/] {nyttFordon.Fordonstyp}\n[PaleGreen3_1]Registreringnummer:[/] {regNr}\n[PaleGreen3_1]Incheckad på plats:[/] {ledigRuta.Plats}");
        }
    }
    public void Utcheckning(ParkeringsGarage garage, AppConfig config)
    {
        string regNr = AngeRegNr();
        ParkeringsRuta ruta = garage.SökaFordonsPlats(regNr);

        if (ruta == null)
        {
            AnsiConsole.MarkupLine("[LightCoral]Fordonet hittades inte.[/]");
            return;
        }
        Fordon fordon = ruta.HittaFordonIRuta(regNr);

        if (fordon == null)
        {
            AnsiConsole.MarkupLine("[LightCoral]Fordon kunde inte hittas i rutan.");
            return;
        }

        PrisLista prisLista = new PrisLista(config);
        decimal kostnad = prisLista.BeräknaKostnad(fordon);

        ruta.TaBortFordon(regNr);

        TimeSpan tidParkerad = RäknaUtParkeringstid(fordon);

        AnsiConsole.MarkupLine("[LightPink3]=== Utcheckning ===[/]");
        AnsiConsole.MarkupLine($"[Cornsilk1]Fordon: {fordon.Fordonstyp}\n" +
            $"RegNr: {regNr}\n" +
            $"Plats:{ruta.Plats}\n" +
            $"Parkerad tid:{tidParkerad.Days} dagar, {tidParkerad.Hours} timmar, {tidParkerad.Minutes} minuter\n" +
            $"Att betala:{kostnad:N2} CZK[/]");
        AnsiConsole.MarkupLine("[PaleGreen3_1]Utcheckning genomförd[/]");
    }
    public string AngeRegNr()
    {
        var regNr = AnsiConsole.Ask<string>("[Cornsilk1]\nVänligen ange registreringsnummer (max 10 tecken):[/]").ToUpper().Trim();
        AnsiConsole.MarkupLine($"Du angav registreringsnummer: [LightSkyBlue1]{regNr}[/]");
        return regNr;
    }
    public void SökaFordonOchPlats(ParkeringsGarage garage)
    {
        string regNr = AngeRegNr();
        ParkeringsRuta ruta = garage.SökaFordonsPlats(regNr);
        if (ruta == null)
        {
            AnsiConsole.MarkupLine("[LightCoral]Fordonet hittades inte.[/]");
            return;
        }
        Fordon fordon = ruta.HittaFordonIRuta(regNr);

        if (fordon == null)
        {
            AnsiConsole.MarkupLine("[LightCoral]Fordon kunde inte hittas i rutan.");
            return;
        }
        TimeSpan tidParkerad = RäknaUtParkeringstid(fordon);
        AnsiConsole.MarkupLine($"[PaleGreen3_1]Fordon:[/] {fordon.Fordonstyp}\n[PaleGreen3_1]RegNr:[/] {regNr}\n[PaleGreen3_1]Plats:[/]{ruta.Plats}\n[PaleGreen3_1]Parkeringstid:[/]{tidParkerad.Days} dagar, {tidParkerad.Hours} timmar, {tidParkerad.Minutes} minuter");

    }
    public void FlyttaFordon(ParkeringsGarage garage)
    {
        string regNr = AngeRegNr();

        int valdPlats = AnsiConsole.Prompt(new TextPrompt<int>($"Vilken plats vill du flytta fordonet till (1-{garage.Rutor.Count})?")
            .Validate(plats =>
            {
                if (plats >= 1 && plats <= garage.Rutor.Count)
                {
                    return ValidationResult.Success();
                }
                return ValidationResult.Error($"[LightCoral]Ogiltigt! Du måste välja en plats mellan 1 och {garage.Rutor.Count}.[/]");
            }));

        Flyttresultat resultat = garage.FlyttaFordon(regNr, valdPlats);

        switch (resultat)
        {
            case Flyttresultat.Lyckats:
                AnsiConsole.MarkupLine($"[PaleGreen3_1]{regNr} har flyttats till plats {valdPlats}.[/]");
                break;
            case Flyttresultat.FordonSaknas:
                AnsiConsole.MarkupLine("[LightCoral]Fordonet hittades inte.[/]");
                break;
            case Flyttresultat.SammaPlats:
                AnsiConsole.MarkupLine("[LightCoral]Fordonet står redan på den platsen.[/]");
                break;
            case Flyttresultat.UtrymmeSaknas:
                AnsiConsole.MarkupLine("[LightCoral]Det finns inte tillräckligt med utrymme på den platsen.[/]");
                break;
            case Flyttresultat.OgiltigPlats:
                AnsiConsole.MarkupLine("[LightCoral]Platsnumret är ogiltigt.[/]");
                break;
        }

    }
    public void VisaParkeringsGarage(ParkeringsGarage garage)
    {
        VisaParkeringsplatser(garage);
        Console.WriteLine();
        VisaStatistik(garage);
    }
    private void VisaStatistik(ParkeringsGarage garage)
    {
        var antalFordon = garage.BeräknaAntalFordon();
        int totaltAntalFordon = antalFordon.Values.Sum();

        string grundText = $"[bold]Parkerade fordon:[/] {totaltAntalFordon}\n[bold]Fordonstyp: [/]\n";

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
            .Header("\n[yellow]Sammanställning[/]")
            .Border(BoxBorder.Rounded)
            .Padding(0, 0);

        AnsiConsole.Write(statistikPanel);

        var förklaringar = new Panel("[IndianRed1]Röd:[/] Upptaget\n[LightGoldenrod2_1]Orange:[/] Delvis upptaget".TrimEnd())
    .Header("\n[yellow]Färgkodning[/]")
    .Border(BoxBorder.Rounded)
    .Padding(0, 0);
        AnsiConsole.Write(förklaringar);
    }
    private void VisaParkeringsplatser(ParkeringsGarage garage)
    {
        var tabell = new Table();
        tabell.Border(TableBorder.None);
        tabell.Title("[LightPink3]=== Prague Parking - Garageöversikt ===[/]");

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
                    //string textIRutan = $"[bold]{ruta.Plats}[/]";
                    string textIRutan;

                    if (ruta.ParkeradeFordon.Count == 0)
                    {
                        textIRutan = "[PaleGreen3_1]Ledig[/]";
                    }
                    else
                    {
                        int upptaget = ruta.ParkeradeFordon.Sum(fordon => fordon.Storlek);
                        string färg;
                        if (upptaget == ruta.MaxKapacitet) { färg = "IndianRed1"; }
                        else { färg = "LightGoldenrod2_1"; }

                        var fordonRader = ruta.ParkeradeFordon
                            .Select(fordon => $"[{färg}]{fordon.Fordonstyp}: {fordon.RegNr}[/]");

                        textIRutan = string.Join("\n", fordonRader);

                    }
                    rad[j] = new Panel(textIRutan)
                         .Border(BoxBorder.Rounded)
                         .Header($"{ruta.Plats}")
                            .PadLeft(0)
                            .PadRight(0)
                            .Expand()
                            .HeaderAlignment(Justify.Center);
                }
            }
            tabell.AddRow(rad);
        }
        AnsiConsole.Write(Align.Left(tabell));
    }
    private TimeSpan RäknaUtParkeringstid(Fordon fordon)
    {
        TimeSpan tidParkerad = DateTime.UtcNow - fordon.AnkomstTid;
        return tidParkerad;
    }
    public static void VisaPrislista()
    {
        var prisLista = new Panel("Friparkering: 10 minuter\n[LightSkyBlue1]Bil:[/] 20 CZK/h\n[LightSkyBlue1]MC:[/] 10 CZK/h\nTimpris - Per påbörjad timme.".TrimEnd())
    .Header("\n[yellow]Prislista[/]")
    .Border(BoxBorder.Rounded)
    .Padding(0, 0);
        AnsiConsole.Write(prisLista);
    }
}
