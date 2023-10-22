using System;
using System.Collections.Generic;
using TheMiddleman.Entity;

class Program
{
    static int GetAmountOfMiddlemen()
    {
        Console.WriteLine("Wieviel Zwischenhändler nehmen teil?");
        return int.Parse(Console.ReadLine() ?? "0");
    }

    static string GetMiddlemanName(int index)
    {
        Console.WriteLine($"Name von Zwischenhändler {index}:");
        return Console.ReadLine() ?? "";
    }

    static string GetCompany(string name)
    {
        Console.WriteLine($"Name der Firma von {name}:");
        return Console.ReadLine() ?? "";
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

    static Middleman InitializeSingleMiddleman(int index)
    {
        string middlemanName = GetMiddlemanName(index);
        string companyName = GetCompany(middlemanName);
        int initialBalance = GetInitialBalance();
        return new Middleman(middlemanName, companyName, initialBalance);
    }

    static List<Middleman> InitializeAllMiddlemen()
    {
        int amountOfMiddlemen = GetAmountOfMiddlemen();
        List<Middleman> middlemen = new List<Middleman>();
        for (int i = 1; i <= amountOfMiddlemen; i++)
        {
            middlemen.Add(InitializeSingleMiddleman(i));
        }
        return middlemen;
    }

    static void DisplayMiddlemanInfo(Middleman middleman, int currentDay)
    {
        Console.WriteLine($"{middleman.Name} von {middleman.Company} | ${middleman.AccountBalance} | Tag {currentDay}");
    }

    static string ReadProductName(string line)
    {
        return line.Substring(8);
    }

    static int ReadProductDurability(string line)
    {
        return int.Parse(line.Substring(14));
    }

    static int ReadProductBasePrice(string line)
    {
        return int.Parse(line.Substring(13));
    }

    static Product CreateProduct(int id, string name, int durability)
    {
        return new Product { Id = id, Name = name, Durability = durability };
    }

    static List<Product> ReadProducts()
    {
        string[] lines = System.IO.File.ReadAllLines("produkte.yml");
        List<Product> products = new List<Product>();
        Product? currentProduct = null;
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
            else if (line.StartsWith("  baseprice: "))
            {
                int basePrice = ReadProductBasePrice(line);
                if (currentProduct != null)
                {
                    currentProduct.BasePrice = basePrice;
                }
            }
        }
        return products;
    }

    static void ShowMenuAndTakeAction(Middleman middleman, ref int currentDay, List<Product> products)
    {
        bool endRound = false;
        while (!endRound)
        {
            DisplayMiddlemanInfo(middleman, currentDay);
            Console.WriteLine("e) Einkaufen");
            Console.WriteLine("v) Verkaufen");
            Console.WriteLine("b) Runde beenden");
            string userChoice = Console.ReadLine() ?? "";
            HandleUserChoice(userChoice, middleman, products, ref endRound);
        }
    }

    static void HandleUserChoice(string choice, Middleman middleman, List<Product> products, ref bool endRound)
    {
        switch (choice)
        {
            case "b":
                endRound = true;
                break;
            case "e":
                ShowShoppingMenu(products, middleman);
                break;
            case "v":
                ShowSellingMenu(middleman);
                break;
            default:
                Console.WriteLine("Ungültige Auswahl. Bitte erneut versuchen.");
                break;
        }
    }

    static void ShowShoppingMenu(List<Product> products, Middleman middleman)
    {
        Console.WriteLine("Verfügbare Produkte:");
        foreach (Product product in products)
        {
            Console.WriteLine($"{product.Id} {product.Name} ({product.Durability} Tage) ${product.BasePrice}/Stück");
        }
        Console.WriteLine("z) Zurück");
        string? userChoice = Console.ReadLine();
        if (userChoice == "z")
        {
            return;
        }
        SelectProductAndPurchase(middleman, userChoice, products);
    }

    static void SelectProductAndPurchase(Middleman middleman ,string? userChoice, List<Product> products)
    {
        int selectedProductId;
        if (!int.TryParse(userChoice, out selectedProductId) || int.Parse(userChoice) <= 0)
        {
            return;
        }
        Product selectedProduct = products.Find(p => p.Id == selectedProductId);
        Console.WriteLine($"Wieviel von {selectedProduct.Name} kaufen?");
        int quantity = int.Parse(Console.ReadLine());
        if (quantity <= 0)
        {
            return;
        }
        ExecutePurchase(middleman, selectedProduct, quantity);
    }

    static void ExecutePurchase(Middleman middleman, Product selectedProduct, int quantity)
    {
        int totalCost = quantity * selectedProduct.BasePrice;
        if (middleman.AccountBalance < totalCost)
        {
            Console.WriteLine("Nicht genügend Geld vorhanden.");
            return;
        }
        middleman.AccountBalance -= totalCost;
        middleman.OwnedProducts.Add(selectedProduct, quantity);
        Console.WriteLine($"Kauf erfolgreich. Neuer Kontostand: ${middleman.AccountBalance}");
    }

    static void ShowSellingMenu(Middleman middleman)
    {
        Console.WriteLine("Produkte im Besitz:");
        int index = 1;
        foreach (var entry in middleman.OwnedProducts)
        {
            Console.WriteLine($"{index}) {entry.Key.Name} ({entry.Value}) ${entry.Key.SellingPrice}/Stück");
            index++;
        }
        Console.WriteLine("z) Zurück");
        string userChoice = Console.ReadLine() ?? "";
        if (userChoice == "z")
        {
            return;
        }
        int selectedProductIndex = int.Parse(userChoice);
        var selectedEntry = middleman.OwnedProducts.ElementAt(selectedProductIndex - 1);
        var selectedProduct = selectedEntry.Key;
        var availableQuantity = selectedEntry.Value;
        Console.WriteLine($"Wieviel von {selectedProduct.Name} verkaufen (max. {availableQuantity})?");
        int quantityToSell = int.Parse(Console.ReadLine() ?? "0");
        if (quantityToSell <= 0 || quantityToSell > availableQuantity)
        {
            Console.WriteLine("Ungültige Menge.");
            return;
        }
        ExecuteSale(middleman, selectedProduct, quantityToSell);
    }

    static void ExecuteSale(Middleman middleman, Product selectedProduct, int quantityToSell)
    {
        middleman.AccountBalance += quantityToSell * selectedProduct.SellingPrice;
        middleman.OwnedProducts[selectedProduct] -= quantityToSell;
        if (middleman.OwnedProducts[selectedProduct] == 0)
        {
            middleman.OwnedProducts.Remove(selectedProduct);
        }
        Console.WriteLine($"Verkauf erfolgreich. Neuer Kontostand: ${middleman.AccountBalance}");
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

    static void SimulateDay(List<Middleman> middlemen, ref int currentDay, List<Product> products)
    {
        foreach (var middleman in middlemen)
        {
            ShowMenuAndTakeAction(middleman, ref currentDay, products);
        }
        RotateMiddlemen(middlemen);
        currentDay++;
    }

    static void Main()
    {
        List<Middleman> middlemen = InitializeAllMiddlemen();
        List<Product> products = ReadProducts();
        int currentDay = 1;
        while (true)
        {
            SimulateDay(middlemen, ref currentDay, products);
        }
    }
}
