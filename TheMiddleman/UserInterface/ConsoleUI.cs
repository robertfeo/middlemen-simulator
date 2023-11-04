using System.Reflection.Metadata;
using TheMiddleman.Entity;

public class ConsoleUI
{
    private readonly MarketService _marketService;

    public ConsoleUI(MarketService marketService)
    {
        _marketService = marketService;
        _marketService.OnDayStart += ShowMenuAndTakeAction;
    }

    public void RunSimulation()
    {
        InitializeMiddlemen();
        while (true)
        {
            _marketService.SimulateDay();
            Console.Clear();
        }
    }

    private int GetAmountOfMiddlemen()
    {
        Console.WriteLine("Wieviel Zwischenhändler nehmen teil?");
        return int.Parse(Console.ReadLine() ?? "0");
    }

    private string GetMiddlemanName(int index)
    {
        Console.WriteLine($"Name von Zwischenhändler {index}:");
        return Console.ReadLine() ?? "";
    }

    private string GetCompany(string name)
    {
        Console.WriteLine($"Name der Firma von {name}:");
        return Console.ReadLine() ?? "";
    }

    private int GetInitialBalance()
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
                return -1;
        }
    }

    private void InitializeMiddlemen()
    {
        Console.Clear();
        int amountOfMiddlemen = GetAmountOfMiddlemen();
        for (int id = 1; id <= amountOfMiddlemen; id++)
        {
            string middlemanName = GetMiddlemanName(id);
            string companyName = GetCompany(middlemanName);
            int initialBalance = GetInitialBalance();
            _marketService.getMiddlemanService().CreateAndStoreMiddleman(middlemanName, companyName, initialBalance);
        }
        Console.Clear();
    }

    private void DisplayMiddlemanInfo(Middleman middleman, int currentDay)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("==========================================");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{middleman.Name} von {middleman.Company}");
        Console.ResetColor();
        Console.WriteLine($"Kontostand: ${middleman.AccountBalance}");
        Console.WriteLine($"Lagerkapazität: {middleman.Warehouse.Values.Sum()}/{Middleman.MaxStorageCapacity}");
        Console.WriteLine($"Tag: {currentDay}");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("==========================================");
        Console.ResetColor();
    }

    private void ShowMenuAndTakeAction(Middleman middleman, int currentDay)
    {
        bool endRound = false;
        while (!endRound)
        {
            DisplayMiddlemanInfo(middleman, currentDay);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Wählen Sie eine Option:");
            Console.ResetColor();
            Console.WriteLine("e) Einkaufen");
            Console.WriteLine("v) Verkaufen");
            Console.WriteLine("b) Runde beenden");
            string userChoice = Console.ReadLine() ?? "";
            HandleUserChoice(userChoice, middleman, ref endRound);
        }
    }

    private void HandleUserChoice(string choice, Middleman middleman, ref bool endRound)
    {
        switch (choice)
        {
            case "b":
                endRound = true;
                break;
            case "e":
                ShowShoppingMenu(middleman);
                break;
            case "v":
                ShowSellingMenu(middleman);
                break;
            default:
                Console.WriteLine("Ungültige Auswahl. Bitte erneut versuchen.");
                break;
        }
    }

    private void ShowShoppingMenu(Middleman middleman)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Verfügbare Produkte:");
        string header = "| ID   | Name                | Haltbarkeit     | Verfügbar        | Preis       |";
        string divider = "|------|---------------------|-----------------|------------------|-------------|";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(header);
        Console.WriteLine(divider);
        Console.ResetColor();
        foreach (Product product in _marketService.getProductService().GetAllProducts())
        {
            string id = product.Id.ToString().PadRight(4);
            string name = product.Name.PadRight(19);
            string durability = $"{product.Durability} Tage".PadRight(15);
            string availableQuantity = $"Verfügbar: {product.AvailableQuantity}".PadRight(16);
            string basePrice = $"${product.BasePrice}/Stück".PadRight(11);
            Console.WriteLine($"| {id} | {name} | {durability} | {availableQuantity} | {basePrice} |");
        }
        Console.ResetColor();
        Console.WriteLine("z) Zurück");
        string? userChoice = Console.ReadLine();
        if (userChoice == "z")
        {
            return;
        }
        SelectProductAndPurchase(middleman, userChoice, _marketService.getProductService().GetAllProducts());
    }

    private void ShowSellingMenu(Middleman middleman)
    {
        Console.WriteLine("Produkte im Besitz:");
        int index = 1;
        foreach (var entry in middleman.Warehouse)
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
        SelectProductAndSell(middleman, userChoice);
    }

    private void SelectProductAndPurchase(Middleman middleman, string? userChoice, List<Product> products)
    {
        int selectedProductId;
        if (!int.TryParse(userChoice, out selectedProductId) || int.Parse(userChoice) <= 0)
        {
            return;
        }

        Product? selectedProduct = products.Find(p => p.Id == selectedProductId);

        if (selectedProduct == null)
        {
            return;
        }
        else
        {
            Console.WriteLine($"Wieviel von {selectedProduct.Name} kaufen?");
            int quantity = int.Parse(Console.ReadLine() ?? "");
            if (quantity <= 0)
            {
                return;
            }
            else
            {
                _marketService.getMiddlemanService().ProcessPurchase(middleman, selectedProduct, quantity);
            }
        }
    }

    private void SelectProductAndSell(Middleman middleman, string userChoice)
    {
        int selectedProductIndex = int.Parse(userChoice);
        var selectedEntry = middleman.Warehouse.ElementAt(selectedProductIndex - 1);
        var selectedProduct = selectedEntry.Key;
        var availableQuantity = selectedEntry.Value;
        Console.WriteLine($"Wieviel von {selectedProduct.Name} verkaufen (max. {availableQuantity})?");
        int quantityToSell = int.Parse(Console.ReadLine() ?? "0");
        if (quantityToSell <= 0 || quantityToSell > availableQuantity)
        {
            Console.WriteLine("Ungültige Menge.");
            return;
        }
        _marketService.getMiddlemanService().ProcessSale(middleman, selectedProduct, quantityToSell);
        Console.WriteLine($"Verkauf erfolgreich. Neuer Kontostand: ${middleman.AccountBalance}");
    }
}
