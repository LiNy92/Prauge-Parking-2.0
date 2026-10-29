using Prague_Parking_Library;
using Spectre.Console;

namespace Prague_Parking_2._0;

public class Meny
{
    public void StartaMeny(AppConfig config)
    {

        var garage = new ParkeringsGarage(config);
        var parkeringsvakt = new ParkeringsVakt();

        var sparadData = DataLagring.LaddaGarage();
        if (sparadData != null && sparadData.Count == garage.Rutor.Count)
        {
            garage.Rutor.Clear();
            garage.Rutor.AddRange(sparadData);
        }

        while (true)
        {
            var menyVal = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Meny - Prauge Parking")
                    .AddChoices("Checka in fordon", "Checka ut fordon", "Söka fordon", "Flytta fordon", "Visa parkeringsgarage", "Avsluta program"));

            AnsiConsole.MarkupLine($"Du har valt: [green]{menyVal}[/]");
            switch (menyVal)
            {
                case "Checka in fordon":
                    {
                        parkeringsvakt.PåbörjaIncheckning(garage, config);
                        break;
                    }
                case "Checka ut fordon":
                    {
                        parkeringsvakt.VerkställUtcheckning(garage, config);
                        break;
                    }
                case "Söka fordon":
                    {
                        string regNr = parkeringsvakt.AngeRegNr();
                        parkeringsvakt.SökaFordonOchPlats(garage);

                        break;
                    }
                case "Flytta fordon":
                    {
                        parkeringsvakt.FlyttaFordon(garage);
                        break;
                    }
                case "Visa parkeringsgarage":
                    {
                        parkeringsvakt.VisaParkeringsGarage(garage);
                        break;
                    }
                case "Avsluta program":
                    {
                        DataLagring.SparaGarage(garage);
                        return; 
                    }
            }
        }
    }
}
