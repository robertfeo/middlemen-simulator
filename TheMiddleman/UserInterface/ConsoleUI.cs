using TheMiddleman.Entity;

public class ConsoleUI
{
    private readonly MarketService _marketService;
    private readonly ProductService _productService;
    private readonly MiddlemanService _middlemanService;

    public ConsoleUI(MarketService marketService, ProductService productService, MiddlemanService middlemanService)
    {
        _marketService = marketService;
        _productService = productService;
        _middlemanService = middlemanService;
    }

    public void Start()
    {
        // Logic to initialize the game, such as loading middlemen and products
    }

    public void SimulateDay(List<Middleman> middlemen, List<Product> products, int currentDay)
    {
        _marketService.SimulateDay(middlemen, products, currentDay);
    }

    private void ShowMenuAndTakeAction(Middleman middleman, ref int currentDay, List<Product> products)
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
            HandleUserChoice(userChoice, middleman, products, ref endRound);
        }
    }

    static void DisplayMiddlemanInfo(Middleman middleman, int currentDay)
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
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Verfügbare Produkte:");
        string header = "| ID   | Name                | Haltbarkeit     | Verfügbar        | Preis       |";
        string divider = "|------|---------------------|-----------------|------------------|-------------|";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(header);
        Console.WriteLine(divider);
        Console.ResetColor();
        foreach (Product product in products)
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
        SelectProductAndPurchase(middleman, userChoice, products);
    }

    static void ShowSellingMenu(Middleman middleman)
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

    static void SelectProductAndPurchase(Middleman middleman, string? userChoice, List<Product> products)
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
                ExecutePurchase(middleman, selectedProduct, quantity);
            }
        }
    }

    private static void SelectProductAndSell(Middleman middleman, string userChoice)
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
        ExecuteSale(middleman, selectedProduct, quantityToSell);
    }

    // Other methods such as DisplayMiddlemanInfo, HandleUserChoice, ShowShoppingMenu, etc.
}
