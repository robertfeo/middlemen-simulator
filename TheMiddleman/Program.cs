using System;
using System.Collections.Generic;
using TheMiddleman.Entity;

class Program
{
    static int GetNumMiddlemen()
    {
        Console.WriteLine("Wieviel Zwischenhändler nehmen teil?");
        return int.Parse(Console.ReadLine() ?? "0");
    }

    static string GetName(int index)
    {
        Console.WriteLine($"Name von Zwischenhändler {index}:");
        return Console.ReadLine() ?? "";
    }

    static string GetCompany(string name)
    {
        Console.WriteLine($"Name der Firma von {name}:");
        return Console.ReadLine() ?? "";
    }

    static Middleman CreateMiddleman(int index)
    {
        string name = GetName(index);
        string company = GetCompany(name);
        int initialBalance = GetInitialBalance();

        return new Middleman { Name = name, Company = company, AccountBalance = initialBalance };
    }

    static int GetInitialBalance()
    {
        Console.WriteLine("Choose a difficulty level (Einfach, Normal, Schwer):");
        string difficulty = Console.ReadLine() ?? "Normal";

        switch (difficulty)
        {
            case "Einfach":
                return 15000;
            case "Normal":
                return 10000;
            case "Schwer":
                return 7000;
            default:
                return 10000;  // Default to Normal
        }
    }

    static List<Middleman> InitializeMiddlemen()
    {
        List<Middleman> middlemen = new List<Middleman>();
        int numMiddlemen = GetNumMiddlemen();

        for (int i = 1; i <= numMiddlemen; i++)
        {
            middlemen.Add(CreateMiddleman(i));
        }

        return middlemen;
    }

    static void DisplayMiddlemanInfo(Middleman middleman, int currentDay)
    {
        Console.WriteLine($"{middleman.Name} von {middleman.Company} | Tag {currentDay}");
        Console.WriteLine("b) Runde beenden");
    }

    static void ShowMenuAndTakeAction(Middleman middleman, ref int currentDay)
    {
        DisplayMiddlemanInfo(middleman, currentDay);
        string option = Console.ReadLine() ?? "";

        if (option == "b")
        {
            // Do nothing, just continue to next middleman
        }
    }

    static void RotateMiddlemen(List<Middleman> middlemen)
    {
        if (middlemen.Count > 1)
        {
            var firstMiddleman = middlemen[0];
            middlemen.RemoveAt(0);
            middlemen.Add(firstMiddleman);
        }
    }

    static void SimulateDay(List<Middleman> middlemen, ref int currentDay)
    {
        foreach (var middleman in middlemen)
        {
            ShowMenuAndTakeAction(middleman, ref currentDay);
        }

        RotateMiddlemen(middlemen);

        currentDay++;
    }

    static void Main()
    {
        List<Middleman> middlemen = InitializeMiddlemen();
        int currentDay = 1;

        while (true)
        {
            SimulateDay(middlemen, ref currentDay);
        }
    }
}
