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
    
}

public class Bil
{

}
public class MC
{

}
public class Buss
{

}
public class Cykel
{

}
