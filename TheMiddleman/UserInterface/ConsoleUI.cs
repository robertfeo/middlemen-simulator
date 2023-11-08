using System.Reflection.Metadata;
using TheMiddleman.Entity;

public class ConsoleUI
{
    private readonly MarketService _marketService;

    public ConsoleUI(MarketService marketService)
    {
        _marketService = marketService;
        _marketService._OnDayStart += ShowMenuAndTakeAction;
        Console.OutputEncoding = System.Text.Encoding.UTF8;
    }

    public void RunSimulation()
    {
        CreateMiddlemen();
        while (true)
        {
            _marketService.SimulateDay();
            Console.Clear();
        }
    }

    private int RequestAmountOfMiddlemen()
    {
        Console.WriteLine("Wieviel Zwischenhändler nehmen teil?");
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int amount) && amount > 0)
            {
                return amount;
            }
            ShowErrorLog("Ungültige Eingabe. Bitte geben Sie eine positive Zahl ein.");
        }
    }

    private string AskForMiddlemanName(int index)
    {
        Console.WriteLine($"Name von Zwischenhändler {index}:");
        string name;
        while (true)
        {
            name = Console.ReadLine()!;
            if (!string.IsNullOrWhiteSpace(name))
            {
                return name;
            }
            ShowErrorLog("Der Name darf nicht leer sein. Bitte erneut versuchen.");
        }
    }

    private string InquireCompany(string name)
    {
        Console.WriteLine($"Name der Firma von {name}:");
        string companyName;
        while (true)
        {
            companyName = Console.ReadLine()!;
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                return companyName;
            }
            ShowErrorLog("Der Name der Firma darf nicht leer sein. Bitte erneut versuchen.");
        }
    }

    private int DetermineInitialBalance()
    {
        Console.WriteLine("Schwierigkeitsgrad auswählen (Einfach, Normal, Schwer):");
        while (true)
        {
            string difficulty = Console.ReadLine()?.ToLower() ?? "";
            switch (difficulty)
            {
                case "einfach":
                    return 15000;
                case "normal":
                    return 10000;
                case "schwer":
                    return 7000;
                default:
                    ShowErrorLog("Ungültige Eingabe. Bitte 'Einfach', 'Normal' oder 'Schwer' eingeben.");
                    break;
            }
        }
    }

    private void CreateMiddlemen()
    {
        Console.Clear();
        int amountOfMiddlemen = RequestAmountOfMiddlemen();
        for (int id = 1; id <= amountOfMiddlemen; id++)
        {
            string middlemanName = AskForMiddlemanName(id);
            string companyName = InquireCompany(middlemanName);
            int initialBalance = DetermineInitialBalance();
            _marketService.MiddlemanService().CreateAndStoreMiddleman(middlemanName, companyName, initialBalance);
        }
        Console.Clear();
    }

    private void DisplayMiddlemanInfo(Middleman middleman, int currentDay)
    {
        char horizontalLine = '\u2550';     // '═' Double horizontal
        char verticalLine = '\u2551';       // '║' Double vertical
        char topLeftCorner = '\u2554';      // '╔' Double down and right
        char topRightCorner = '\u2557';     // '╗' Double down and left
        char bottomLeftCorner = '\u255A';   // '╚' Double up and right
        char bottomRightCorner = '\u255D';  // '╝' Double up and left
        string topBorder = topLeftCorner + new string(horizontalLine, 42) + topRightCorner;
        string bottomBorder = bottomLeftCorner + new string(horizontalLine, 42) + bottomRightCorner;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(topBorder);
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{verticalLine} {middleman.Name} von {middleman.Company}".PadRight(43) + verticalLine);
        Console.ResetColor();
        Console.WriteLine($"{verticalLine} Kontostand: ${middleman.AccountBalance}".PadRight(43) + verticalLine);
        Console.WriteLine($"{verticalLine} Lagerkapazität: {middleman.Warehouse.Values.Sum()}/{middleman.MaxStorageCapacity}".PadRight(43) + verticalLine);
        Console.WriteLine($"{verticalLine} Tag: {currentDay}".PadRight(43) + verticalLine);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(bottomBorder);
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
            Console.WriteLine("t) Lager vergrößern");
            Console.WriteLine("b) Runde beenden");
            ManageUserInteraction(Console.ReadLine()!, middleman, ref endRound);
        }
    }

    private void ManageUserInteraction(string userInput, Middleman middleman, ref bool endRound)
    {
        switch (userInput)
        {
            case "b":
                EndRound(ref endRound);
                break;
            case "e":
                InitiateShopping(middleman);
                break;
            case "v":
                InitiateSelling(middleman);
                break;
            case "t":
                InitiateWarehouseExpansion(middleman);
                break;
            default:
                NotifyInvalidChoice();
                break;
        }
    }

    private void EndRound(ref bool endRound)
    {
        Console.Clear();
        endRound = true;
    }

    private void InitiateShopping(Middleman middleman)
    {
        Console.Clear();
        ShowShoppingMenu(middleman);
    }

    private void InitiateSelling(Middleman middleman)
    {
        Console.Clear();
        ShowSellingMenu(middleman);
    }

    private void InitiateWarehouseExpansion(Middleman middleman)
    {
        Console.Clear();
        ShowExtendingWarehouse(middleman);
    }

    private void NotifyInvalidChoice()
    {
        Console.Clear();
        ShowErrorLog("Error: Ungültige Auswahl. Bitte erneut versuchen.");
    }

    private void ShowExtendingWarehouse(Middleman middleman)
    {
        Console.WriteLine("Um wie viel Einheiten möchten Sie das Lager vergrößern? ($50 pro Einheit)");
        int increaseAmount;
        if (!int.TryParse(Console.ReadLine(), out increaseAmount) || increaseAmount <= 0)
        {
            ShowErrorLog("Vergrößerung des Lagers abgebrochen.");
            return;
        }
        int costForIncrease = increaseAmount * 50;
        if (middleman.AccountBalance < costForIncrease)
        {
            ShowErrorLog($"Nicht genug Geld für die Vergrößerung des Lagers vorhanden.\nVerfügbares Guthaben: ${middleman.AccountBalance}");
            return;
        }
        middleman.AccountBalance -= costForIncrease;
        _marketService.MiddlemanService().IncreaseWarehouseCapacity(middleman, increaseAmount);
        ShowMenuAndTakeAction(middleman, _marketService._currentDay);
    }

    private void ShowShoppingMenu(Middleman middleman)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Verfügbare Produkte:");
        string header = "| ID   | Name                | Haltbarkeit     | Verfügbar        | Preis       |";
        string divider = "|------|---------------------|-----------------|------------------|-------------|";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(header);
        Console.WriteLine(divider);
        Console.ResetColor();
        ShowAllProducts();
        Console.ResetColor();
        Console.WriteLine("z) Zurück");
        string userChoice = Console.ReadLine()!;
        if (userChoice == "z")
        {
            Console.Clear();
            return;
        }
        SelectProductAndPurchase(middleman, userChoice, _marketService.ProductService().GetAllProducts());
        ShowShoppingMenu(middleman);
    }

    private void ShowAllProducts()
    {
        foreach (Product product in _marketService.ProductService().GetAllProducts())
        {
            string id = product.Id.ToString().PadRight(4);
            string name = product.Name.PadRight(19);
            string durability = $"{product.Durability} Tage".PadRight(15);
            string availableQuantity = $"Verfügbar: {product.AvailableQuantity}".PadRight(16);
            string purchasePrice = $"${product.PurchasePrice}/Stück".PadRight(11);
            Console.WriteLine($"| {id} | {name} | {durability} | {availableQuantity} | {purchasePrice} |");
        }
    }

    private int ChooseQuantityForPurchase(Product selectedProduct)
    {
        Console.WriteLine($"Wieviel von {selectedProduct.Name} kaufen?");
        string userInput = Console.ReadLine() ?? "";
        if (!int.TryParse(userInput, out int quantity) || quantity <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ungültige Menge. Bitte erneut versuchen.");
            Console.ResetColor();
            return -1;
        }
        return quantity;
    }

    private void SelectProductAndPurchase(Middleman middleman, string userChoice, List<Product> products)
    {
        if (!int.TryParse(userChoice, out int selectedProductId) || selectedProductId <= 0)
        {
            Console.Clear();
            ShowErrorLog("Ungültige Auswahl. Bitte erneut versuchen.");
            return;
        }
        Product? selectedProduct = products.Find(p => p.Id == selectedProductId);
        if (selectedProduct == null || selectedProduct.AvailableQuantity == 0)
        {
            Console.Clear();
            ShowErrorLog("Dieses Produkt ist nicht mehr verfügbar. Bitte erneut versuchen.\n");
            return;
        }
        int quantity = ChooseQuantityForPurchase(selectedProduct);
        if (quantity == -1 || quantity > selectedProduct.AvailableQuantity)
        {
            Console.Clear();
            ShowErrorLog("Beim letzten Kauf wurde eine ungültige Menge eingegeben. Bitte erneut versuchen.\n");
            return;
        }
        _marketService.MiddlemanService().Purchase(middleman, selectedProduct, quantity, out string errorLog);
        Console.Clear();
        if (!string.IsNullOrEmpty(errorLog))
        {
            ShowErrorLog(errorLog + "\n");
            return;
        }
    }

    private void ShowSellingMenu(Middleman middleman)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Produkte im Besitz:");
        string header = "| ID   | Name                | Menge            | Verkaufspreis    |";
        string divider = "|------|---------------------|------------------|------------------|";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(header);
        Console.WriteLine(divider);
        Console.ResetColor();
        ShowOwnedProducts(middleman);
        Console.WriteLine("z) Zurück");
        string userChoice = Console.ReadLine()!;
        if (userChoice == "z")
        {
            Console.Clear();
            return;
        }
        else
        {
            SelectProductAndSell(middleman, userChoice);
            ShowSellingMenu(middleman);
        }
    }

    private int ChooseQuantityForSale(KeyValuePair<Product, int> selectedEntry)
    {
        var (product, availableQuantity) = selectedEntry;
        Console.WriteLine($"Wieviel von {product.Name} verkaufen (max. {availableQuantity})?");
        string userInput = Console.ReadLine() ?? "";
        if (!int.TryParse(userInput, out int quantityToSell) || quantityToSell <= 0 || quantityToSell > availableQuantity)
        {
            ShowErrorLog("Ungültige Menge. Bitte erneut versuchen.");
            return -1;
        }
        return quantityToSell;
    }

    private void ShowOwnedProducts(Middleman middleman)
    {
        int index = 1;
        foreach (var entry in middleman.Warehouse)
        {
            string id = index.ToString().PadRight(4);
            string name = entry.Key.Name.PadRight(19);
            string quantity = entry.Value.ToString().PadRight(16);
            string sellingPrice = $"${entry.Key.SellingPrice}/Stück".PadRight(16);
            Console.WriteLine($"| {id} | {name} | {quantity} | {sellingPrice} |");
            index++;
        }
    }

    private void SelectProductAndSell(Middleman middleman, string userChoice)
    {
        if (!int.TryParse(userChoice, out int selectedProductIndex) || selectedProductIndex <= 0 || selectedProductIndex > middleman.Warehouse.Count)
        {
            Console.Clear();
            ShowErrorLog("Ungültige Auswahl. Bitte erneut versuchen.");
            return;
        }
        var selectedEntry = middleman.Warehouse.ElementAt(selectedProductIndex - 1);
        int quantityToSell = ChooseQuantityForSale(selectedEntry);
        if (quantityToSell == -1)
        {
            Console.Clear();
            ShowErrorLog("Beim letzten Verkauf wurde eine ungültige Menge eingegeben. Bitte erneut versuchen.\n");
            return;
        }
        _marketService.MiddlemanService().Sale(middleman, selectedEntry.Key, quantityToSell, out string errorLog);
        Console.Clear();
        if (!string.IsNullOrEmpty(errorLog))
        {
            ShowErrorLog(errorLog + "\n");
            return;
        }
    }

    private void ShowErrorLog(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Error: " + message + "\n");
        Console.ResetColor();
    }
}