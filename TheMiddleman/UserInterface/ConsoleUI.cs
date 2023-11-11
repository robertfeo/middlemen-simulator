using System.Reflection.Metadata;
using TheMiddleman.Entity;

public class ConsoleUI
{
    private readonly MarketService _marketService;

    public ConsoleUI(MarketService marketService)
    {
        _marketService = marketService;
        _marketService._OnDayStart += ShowMenuAndTakeAction;
        _marketService._OnBankruptcy += HandleBankruptcy;
        _marketService._OnEndOfGame += ShowEndOfGame;
        Console.OutputEncoding = System.Text.Encoding.UTF8;
    }

    private void ShowEndOfGame(List<Middleman> middlemen)
    {
        string message;
        if (middlemen.Count == 0)
        {
            message = "Simulation beendet. Alle Zwischenhändler sind bankrott gegangen";
        }
        else
        {
            message = "Simulation beendet. Hier ist die Rangliste:";
            PrintMessageInFrame(ConsoleColor.Magenta, message);
            foreach (var middleman in middlemen)
            {
                PrintMessageInFrame(ConsoleColor.Magenta, $"{middleman.Name}: ${middleman.AccountBalance}");
            }
            return;
        }
        PrintMessageInFrame(ConsoleColor.Magenta, message);
    }

    private void HandleBankruptcy(Middleman middleman)
    {
        if (_marketService.CheckIfMiddlemanIsLastBankroped(middleman))
        {
            return;
        }
        else
        {
            PrintMessageInFrame(ConsoleColor.Magenta, $"{middleman.Name} ist Bankrott gegangen und wird ausgeschlossen.");
        }
    }

    private int RequestSimulationDuration()
    {
        Console.WriteLine("Wie lange soll die Simulation laufen? (Anzahl der Tage)");
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int days) && days > 0)
            {
                return days;
            }
            ShowErrorLog("Ungültige Eingabe. Bitte geben Sie eine positive Zahl ein.");
        }
    }

    public void RunSimulation()
    {
        int simulationLength = RequestSimulationDuration();
        _marketService.SetSimulationDuration(simulationLength);  // Add this line
        CreateMiddlemen();
        while (_marketService._currentDay <= simulationLength)
        {
            _marketService.SimulateDay();
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
        int amountOfMiddlemen = RequestAmountOfMiddlemen();
        for (int id = 1; id <= amountOfMiddlemen; id++)
        {
            string middlemanName = AskForMiddlemanName(id);
            string companyName = InquireCompany(middlemanName);
            int initialBalance = DetermineInitialBalance();
            _marketService.MiddlemanService().CreateAndStoreMiddleman(middlemanName, companyName, initialBalance);
        }
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
            Console.WriteLine("e) Einkaufen");
            Console.WriteLine("v) Verkaufen");
            Console.WriteLine("t) Lager vergrößern");
            Console.WriteLine("b) Runde beenden");
            Console.ResetColor();
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
        endRound = true;
    }

    private void InitiateShopping(Middleman middleman)
    {
        ShowShoppingMenu(middleman);
    }

    private void InitiateSelling(Middleman middleman)
    {
        ShowSellingMenu(middleman);
    }

    private void InitiateWarehouseExpansion(Middleman middleman)
    {
        ShowExtendingWarehouse(middleman);
    }

    private void NotifyInvalidChoice()
    {
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

    private void ShowAllProducts()
    {
        char verticalLine = '\u2551'; // '║' Double vertical
        foreach (Product product in _marketService.ProductService().GetAllProducts())
        {
            string id = product.Id.ToString().PadRight(4);
            string name = product.Name.PadRight(20);
            string durability = $"{product.Durability} Tage".PadRight(16);
            string availableQuantity = $"Verfügbar: {product.AvailableQuantity}".PadRight(18);
            string purchasePrice = $"${product.PurchasePrice}/Stück".PadRight(13);
            Console.WriteLine($"{verticalLine} {id} {verticalLine} {name} {verticalLine} {durability} {verticalLine} {availableQuantity} {verticalLine} {purchasePrice} {verticalLine}");
        }
    }

    private void ShowShoppingMenu(Middleman middleman)
    {
        char horizontalLine = '\u2550';     // '═' Double horizontal
        char verticalLine = '\u2551';       // '║' Double vertical
        char topLeftCorner = '\u2554';      // '╔' Double down and right
        char topRightCorner = '\u2557';     // '╗' Double down and left
        char bottomLeftCorner = '\u255A';   // '╚' Double up and right
        char bottomRightCorner = '\u255D';  // '╝' Double up and left
        char connector = '\u256C';          // '╬' Double vertical and horizontal
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Verfügbare Produkte:");
        string header = $"{verticalLine} ID   {verticalLine} Name                 {verticalLine} Haltbarkeit      {verticalLine} Verfügbar          {verticalLine} Preis         {verticalLine}";
        string divider = $"{connector}══════{connector}══════════════════════{connector}══════════════════{connector}════════════════════{connector}═══════════════{connector}";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(topLeftCorner + new string(horizontalLine, header.Length - 2) + topRightCorner);
        Console.WriteLine(header);
        Console.WriteLine(divider);
        Console.ResetColor();
        ShowAllProducts();
        Console.WriteLine(bottomLeftCorner + new string(horizontalLine, header.Length - 2) + bottomRightCorner);
        Console.ResetColor();
        Console.WriteLine("z) Zurück");
        string userChoice = Console.ReadLine()!;
        if (userChoice == "z")
        {
            return;
        }
        SelectProductAndPurchase(middleman, userChoice, _marketService.ProductService().GetAllProducts());
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
            ShowErrorLog("Ungültige Auswahl. Bitte erneut versuchen.");
            return;
        }
        Product? selectedProduct = products.Find(p => p.Id == selectedProductId);
        if (selectedProduct == null || selectedProduct.AvailableQuantity == 0)
        {
            ShowErrorLog("Dieses Produkt ist nicht mehr verfügbar. Bitte erneut versuchen.\n");
            return;
        }
        int quantity = ChooseQuantityForPurchase(selectedProduct);
        if (quantity == -1 || quantity > selectedProduct.AvailableQuantity)
        {
            ShowErrorLog("Beim letzten Kauf wurde eine ungültige Menge eingegeben. Bitte erneut versuchen.\n");
            return;
        }
        _marketService.MiddlemanService().Purchase(middleman, selectedProduct, quantity, out string errorLog);
        if (!string.IsNullOrEmpty(errorLog))
        {
            ShowErrorLog(errorLog + "\n");
            return;
        }
        Console.WriteLine($"Sie haben {quantity}x {selectedProduct.Name} gekauft.");
    }

    private void ShowSellingMenu(Middleman middleman)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Produkte im Besitz:");
        Console.WriteLine("Budget hat nicht gereicht für ein besseres Design🥱");
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
            return;
        }
        else
        {
            SelectProductAndSell(middleman, userChoice);
            return;
        }
    }

    private void SelectProductAndSell(Middleman middleman, string userChoice)
    {
        if (!int.TryParse(userChoice, out int selectedProductIndex) || selectedProductIndex <= 0 || selectedProductIndex > middleman.Warehouse.Count)
        {
            ShowErrorLog("Ungültige Auswahl. Bitte erneut versuchen.");
            return;
        }
        var selectedEntry = middleman.Warehouse.ElementAt(selectedProductIndex - 1);
        int quantityToSell = ChooseQuantityForSale(selectedEntry);
        if (quantityToSell == -1)
        {
            ShowErrorLog("Beim letzten Verkauf wurde eine ungültige Menge eingegeben. Bitte erneut versuchen.\n");
            return;
        }
        _marketService.MiddlemanService().Sale(middleman, selectedEntry.Key, quantityToSell, out string errorLog);
        if (!string.IsNullOrEmpty(errorLog))
        {
            ShowErrorLog(errorLog + "\n");
            return;
        }
        Console.WriteLine($"Sie haben {quantityToSell}x {selectedEntry.Key.Name} verkauft.");
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

    private void ShowErrorLog(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Error: " + message + "\n");
        Console.ResetColor();
    }

    private void PrintMessageInFrame(ConsoleColor color, string message)
    {
        int messageLength = message.Length + 2; // Add padding for the message
        char horizontalLine = '\u2550';     // '═' Double horizontal
        char verticalLine = '\u2551';       // '║' Double vertical
        char topLeftCorner = '\u2554';      // '╔' Double down and right
        char topRightCorner = '\u2557';     // '╗' Double down and left
        char bottomLeftCorner = '\u255A';   // '╚' Double up and right
        char bottomRightCorner = '\u255D';  // '╝' Double up and left
        Console.ForegroundColor = color;
        Console.Write(topLeftCorner);
        Console.Write(new string(horizontalLine, messageLength));
        Console.WriteLine(topRightCorner);
        Console.Write(verticalLine);
        Console.Write($" {message} ");
        Console.WriteLine(verticalLine);
        Console.Write(bottomLeftCorner);
        Console.Write(new string(horizontalLine, messageLength));
        Console.WriteLine(bottomRightCorner);
        Console.ResetColor();
    }
}