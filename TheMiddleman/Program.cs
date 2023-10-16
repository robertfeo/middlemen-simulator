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
        Console.WriteLine("Schwierigkeitsgrad auswählen (Einfach, Normal, Schwer):");
        string difficulty = Console.ReadLine()?.ToLower() ?? "normal";

        switch (difficulty)
        {
            case "einfach":
                return 15000;
            case "normal":
                return 10000;
            case "schwer":
                return 7000;
            default:
                return 10000;
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
        Console.WriteLine($"{middleman.Name} von {middleman.Company} | ${middleman.AccountBalance} | Tag {currentDay}");
        Console.WriteLine("b) Runde beenden");
    }


    static string ReadProductName(string line)
    {
        return line.Substring(8);
    }

    static int ReadProductDurability(string line)
    {
        return int.Parse(line.Substring(14));
    }

    static Product CreateProduct(int id, string name, int durability)
    {
        return new Product { Id = id, Name = name, Durability = durability };
    }

    static List<Product> ReadProducts()
    {
        string[] lines = System.IO.File.ReadAllLines("produkte.yml");
        List<Product> products = new List<Product>();
        Product currentProduct = null;
        int idCounter = 1;

        foreach (var line in lines)
        {
            if (line.StartsWith("- name: "))
            {
                string name = ReadProductName(line);
                currentProduct = CreateProduct(idCounter++, name, 0);
            }
            else if (line.StartsWith("  durability: "))
            {
                if (currentProduct != null)
                {
                    int durability = ReadProductDurability(line);
                    currentProduct.Durability = durability;
                    products.Add(currentProduct);
                }
            }
        }

        return products;
    }

    static void ShowMenuAndTakeAction(Middleman middleman, ref int currentDay)
    {
        DisplayMiddlemanInfo(middleman, currentDay);
        Console.WriteLine("e) Einkaufen");
        Console.WriteLine("b) Runde beenden");
        string option = Console.ReadLine() ?? "";

        if (option != "b")
        {
            Console.WriteLine("Ungültige Eingabe!");
            ShowMenuAndTakeAction(middleman, ref currentDay);
        }
        else if (option == "e")  // New block
        {
            Console.WriteLine("Verfügbare Produkte:");
            foreach (var product in middleman.Products)
            {
                Console.WriteLine($"{product.Id}) {product.Name} ({product.Durability} Tage)");
            }
            Console.WriteLine("z) Zurück");
            if (Console.ReadLine() == "z")
            {
                // Go back to the main menu
            }
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
