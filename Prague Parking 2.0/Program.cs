using Spectre.Console;

namespace Prague_Parking_2._0;

internal class Program
{
    static void Main(string[] args)
    {

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
                        //metod incheckning ??
                        var fordonstyp = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                            .Title("Vilket fordon önskar du checka in?")
                            .AddChoices("Bil", "MC", "Buss", "Cykel", "Återgå till meny"));

                        if (fordonstyp == "Återgå till meny") continue;

                        var regNr = AnsiConsole.Ask<string>("[green]\nVänligen ange registreringsnummer (max 10 tecken):[/]").ToUpper().Trim();
                        AnsiConsole.MarkupLine($"Du angav registreringsnummer: [red]{regNr}[/]");
                        if (AnsiConsole.Confirm("Vill du checka in angivet fordon."))
                        {
                            Fordon nyttFordon = null;
                            switch (fordonstyp)
                            {
                                case "Bil":
                                    nyttFordon = new Bil(regNr);        //Ska läggas till i listan
                                    break;
                                case "MC":
                                    nyttFordon = new MC(regNr);         //Add till listan
                                    break;
                            }
                            AnsiConsole.MarkupLine($"[green]Din {nyttFordon.Fordonstyp} med registreringsnummer: {nyttFordon.RegNr} har checkat in på plats: ???.[/]");
                        }
                        else continue;

                        break;
                    }
                case "Checka ut fordon":
                    {
                        break;
                    }
                case "Söka fordon":
                {
                    break;
                }
                case "Flytta fordon":
                    {
                        break;
                    }
                case "Visa prislista":
                    {  break;}
                case "Visa parkeringsgarage":
                    { break; }
                case "Avsluta program":
                    { return; }
            }
        }

    }

}








    //string AngeRegNummer()
    //{
    //    while (true)
    //    {
    //        string regNr = AnsiConsole.Ask<string>("\nVänligen ange regnr (max 10 tecken): ");
    //        string regNr = Console.ReadLine().ToUpper().Trim(); //Omvandlar till stora bokstäver här direkt och tar bort eventuella mellanslag

    //        if (regNr.Length > 0 && regNr.Length <= 10)
    //        {
    //            return regNr;
    //        }
    //        Console.WriteLine("Ogiltig inmatning. Registeringsnumret får ej vara tomt eller innehålla fler än 10 tecken. Försök igen.");
    //    }
    //}




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
public class Buss : Fordon
{
    public Buss(string regNr) : base("Buss", regNr)
    {

    }
}
public class Cykel : Fordon
{
    public Cykel(string regNr) : base("Cykel", regNr)
    {

    }
}

public class ParkeringsGarage
{
    public List<int> ParkeringsPlats { get; set; } = new List<int>();

    public ParkeringsGarage()
    {
        for (int i = 0; i < 100; i++)
        {

        }
    }
}


public class ParkeringsRuta : ParkeringsGarage
{

}