using Prague_Parking_Library;
using Spectre.Console;

namespace Prague_Parking_2._0;

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
