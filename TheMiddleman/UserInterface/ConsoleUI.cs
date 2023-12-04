using System.Reflection.Metadata;
using TheMiddleman.Entity;
using Spectre.Console;

public class ConsoleUI
{
    private readonly MarketService _marketService;
    private bool _dailyReportShown = false;

    public ConsoleUI(MarketService marketService)
    {
        _marketService = marketService;
    }

    public void StartSimulation()
    {
        InitializeConsoleUI();
        RequestSimulationDuration();
        ShowCreationMiddlemen();
        _marketService.RunSimulation();
    }

    private void InitializeConsoleUI()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        try
        {
            AnsiConsole.Write(new FigletText("The Middleman").LeftJustified().Color(Color.Red));
            var rule = new Rule("[lime]Drücken Sie eine beliebige Taste, um fortzufahre...[/]");
            rule.LeftJustified();
            AnsiConsole.Write(rule);
        }
        catch (Exception)
        {
            Console.WriteLine("Willkommen bei The Middleman!");
            Console.WriteLine("Drücken Sie eine beliebige Taste, um fortzufahre...");
        }
        Console.ReadKey();
        BindFunctionsFromOutsideClass();
    }

    private void BindFunctionsFromOutsideClass()
    {
        _marketService._OnDayStart += ShowMenuAndTakeAction;
        _marketService._OnDayChange += ShowCurrentDay;
        _marketService._OnBankruptcy += ShowMiddlemanBankroped;
        _marketService._OnEndOfGame += ShowEndOfGame;
    }

    private (int idWidth, int nameWidth, int durabilityWidth, int availableWidth, int priceWidth) CalculateColumnWidths(List<Product> products)
    {
        int idWidth = products.Max(p => p.Id.ToString().Length) + 4;
        int nameWidth = products.Max(p => p.Name.Length) + 4;
        int durabilityWidth = products.Max(p => $"{p.Durability} Tage".Length) + 4;
        int availableWidth = products.Max(p => $"Verfügbar: {p.AvailableQuantity}".Length) + 4;
        int priceWidth = products.Max(p => $"${p.PurchasePrice}/Stück".Length) + 4;
        return (idWidth, nameWidth, durabilityWidth, availableWidth, priceWidth);
    }

    private (int idWidth, int nameWidth, int quantityWidth, int priceWidth) CalculateSellingColumnWidths(List<Product> products)
    {
        int idWidth = products.Max(p => p.Id.ToString().Length) + 4;
        int nameWidth = products.Max(p => p.Name.Length) + 4;
        int quantityWidth = "Menge".Length + 4;
        int priceWidth = products.Max(p => $"${p.SellingPrice}/Stück".Length) + 4;
        return (idWidth, nameWidth, quantityWidth, priceWidth);
    }

    private void ShowEndOfGame(List<Middleman> middlemen)
    {
        string message;
        if (middlemen.Count == 0)
        {
            message = "Simulation beendet. Alle Zwischenhändler sind bankrott gegangen";
            PrintMessageInFrame(ConsoleColor.Magenta, message);
        }
        else
        {
            message = "Simulation beendet. Hier ist die Rangliste:";
            PrintMessageInFrame(ConsoleColor.Magenta, message);
            int rank = 1;
            foreach (var middleman in middlemen)
            {
                PrintMessageInFrame(ConsoleColor.Magenta, $"Platz {rank}: {middleman.Name} - Kontostand: ${middleman.AccountBalance}");
                rank++;
            }
            return;
        }
    }

    private void ShowMiddlemanBankroped(Middleman middleman, Exception ex)
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

    private void RequestSimulationDuration()
    {
        Console.WriteLine("Wie lange soll die Simulation laufen? (Anzahl der Tage)");
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int days) && days > 0)
            {
                _marketService.SetSimulationDuration(days);
                break;
            }
            else
            {
                ShowErrorLog("Ungültige Eingabe. Bitte geben Sie eine positive Zahl ein.");
            }
        }
    }

    private int RequestAmountOfMiddlemen()
    {
        Console.WriteLine("Wieviel Zwischenhändler nehmen teil?");
        if (int.TryParse(Console.ReadLine(), out int amount) && amount > 0)
        {
            return amount;
        }
        else
        {
            ShowErrorLog("Ungültige Eingabe. Bitte geben Sie eine positive Zahl ein.");
            return RequestAmountOfMiddlemen();
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
        ShowMessage("Schwierigkeitsgrad auswählen (Einfach, Normal, Schwer):");
        while (true)
        {
            switch (Console.ReadLine()?.ToLower() ?? "")
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

    private void ShowCreationMiddlemen()
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
        ShowMessage(topBorder);
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Green;
        ShowMessage($"{verticalLine} {middleman.Name} von {middleman.Company}".PadRight(43) + verticalLine);
        Console.ResetColor();
        ShowMessage($"{verticalLine} Kontostand: ${middleman.AccountBalance}".PadRight(43) + verticalLine);
        ShowMessage($"{verticalLine} Lagerkapazität: {middleman.Warehouse.Values.Sum()}/{middleman.MaxStorageCapacity}".PadRight(43) + verticalLine);
        ShowMessage($"{verticalLine} Tag: {currentDay}".PadRight(43) + verticalLine);
        Console.ForegroundColor = ConsoleColor.Cyan;
        ShowMessage(bottomBorder);
        Console.ResetColor();
    }

    /* private void ShowDailyReport(Middleman middleman)
    {
        if (_dailyReportShown)
        {
            return;
        }
        else
        {
            Console.WriteLine("\nTagesbericht für " + middleman.Name);
            Console.WriteLine("Kontostand zu Beginn des letzten Tages: $" + middleman.PreviousDayBalance);
            Console.WriteLine("Ausgaben für Einkäufe: $" + middleman.DailyExpenses);
            Console.WriteLine("Einnahmen aus Verkäufen: $" + middleman.DailyEarnings);
            Console.WriteLine("Lagerkosten: $" + middleman.DailyStorageCosts);
            Console.WriteLine("Aktueller Kontostand zu Beginn des Tages: $" + middleman.AccountBalance);
            Console.WriteLine("\nDrücken Sie Enter, um fortzufahren...");
            Console.ReadLine();
            _dailyReportShown = true;
            return;
        }
    } */

    private void ShowDailyReport(Middleman middleman)
    {
        if (_dailyReportShown)
        {
            return;
        }
        else
        {
            var table = new Table();

            table.AddColumn(new TableColumn("Kategorie").Centered());
            table.AddColumn(new TableColumn("Betrag").Centered());

            table.AddRow("Kontostand zu Beginn des letzten Tages", $"${middleman.PreviousDayBalance}");
            table.AddRow("Ausgaben für Einkäufe", $"${middleman.DailyExpenses}");
            table.AddRow("Einnahmen aus Verkäufen", $"${middleman.DailyEarnings}");
            table.AddRow("Lagerkosten", $"${middleman.DailyStorageCosts}");
            table.AddRow("Aktueller Kontostand zu Beginn des Tages", $"${middleman.AccountBalance}");

            table.Border(TableBorder.Rounded);
            table.BorderColor(Color.Green);
            table.Title($"Tagesbericht für {middleman.Name}");

            AnsiConsole.Write(table);

            Console.WriteLine("\nDrücken Sie Enter, um fortzufahren...");
            Console.ReadLine();
            _dailyReportShown = true;
            return;
        }
    }


    private void ShowMenuAndTakeAction(Middleman middleman, int currentDay)
    {
        ShowDailyReport(middleman);
        _marketService.MiddlemanService().ResetDailyReport(middleman);
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
        _dailyReportShown = false;
    }

    private void ManageUserInteraction(string userInput, Middleman middleman, ref bool endRound)
    {
        switch (userInput)
        {
            case "b":
                EndRound(ref endRound);
                break;
            case "e":
                ShowShopping(middleman);
                break;
            case "v":
                ShowSelling(middleman);
                break;
            case "t":
                ShowWarehouseExpansion(middleman);
                break;
            default:
                NotifyInvalidMenuChoice();
                break;
        }
    }

    private void EndRound(ref bool endRound)
    {
        endRound = true;
    }

    private void ShowShopping(Middleman middleman)
    {
        ShowShoppingMenu();
        Console.WriteLine("z) Zurück");
        string userInput = GetUserInput();
        if (userInput == "z")
        {
            return;
        }
        else
        {
            InitiatePurchase(middleman, userInput);
        }
    }

    private void InitiatePurchase(Middleman middleman, string userInput)
    {
        try
        {
            Product selectedProduct = _marketService.ProductService().FindProductById(int.Parse(userInput))!;
            int quantity = int.Parse(AskUserForInput("Wie viel von " + selectedProduct.Name + " möchten Sie kaufen?"));
            _marketService.MiddlemanService().PurchaseProduct(middleman, selectedProduct, quantity);
            ShowMessage($"Sie haben {quantity}x {selectedProduct.Name} gekauft.");
        }
        catch (WarehouseCapacityExceededException ex) { ShowErrorLog(ex.Message); }
        catch (InsufficientFundsException ex) { ShowErrorLog(ex.Message); }
        catch (ProductNotFoundException ex) { ShowErrorLog(ex.Message); }
        catch (FormatException) { ShowErrorLog("Ungueltige Eingabe."); }
        catch (Exception ex) { ShowErrorLog(ex.Message); }
    }

    private void InitiateSelling(Middleman middleman, string userInput)
    {
        try
        {
            var ownedProducts = _marketService.MiddlemanService().GetOwnedProducts(middleman);
            if (!int.TryParse(userInput, out int selectedProductId) || selectedProductId <= 0)
            {
                ShowErrorLog("Ungültige Eingabe!");
                return;
            }
            int index = selectedProductId - 1;
            Product selectedProduct = middleman.Warehouse.ElementAt(index).Key;
            if (selectedProduct == null)
            {
                ShowErrorLog("Das ausgewählte Produkt existiert nicht im Lager.");
                return;
            }
            string quantityInput = AskUserForInput($"Wie viele Einheiten von {selectedProduct.Name} möchten Sie verkaufen?");
            int quantityToSell = int.Parse(quantityInput);
            _marketService.MiddlemanService().SellProduct(middleman, selectedProduct, quantityToSell);
            ShowMessage($"Erfolgreich {quantityToSell} Einheiten von {selectedProduct.Name} verkauft.");
        }
        catch (FormatException) { ShowErrorLog("Ungültige Eingabe."); }
        catch (ProductNotAvailableException ex) { ShowErrorLog(ex.Message); }
        catch (Exception ex) { ShowErrorLog(ex.Message); }
    }

    private void ShowSelling(Middleman middleman)
    {
        ShowSellingMenu(middleman);
        string userChoice = GetUserInput();
        if (userChoice == "z")
        {
            return;
        }
        else
        {
            try
            {
                InitiateSelling(middleman, userChoice);
            }
            catch (UserInputException ex)
            {
                ShowErrorLog(ex.Message);
            }
        }
    }

    private void ShowWarehouseExpansion(Middleman middleman)
    {
        try
        {
            ShowMessage("Um wie viele Einheiten möchten Sie das Lager erweitern? (50 $ pro Einheit)");
            int increaseAmount = int.Parse(GetUserInput());
            _marketService.MiddlemanService().IncreaseWarehouseCapacity(middleman, increaseAmount);
        }
        catch (InsufficientFundsException ex)
        {
            ShowErrorLog(ex.Message);
        }
        catch (FormatException)
        {
            ShowErrorLog("Ungültige Eingabe. Bitte geben Sie eine positive Zahl ein.");
        }
    }

    private void NotifyInvalidMenuChoice()
    {
        ShowErrorLog("Ungültige Auswahl. Bitte erneut versuchen.");
    }

    private void ShowAllProducts()
    {
        List<Product> products = _marketService.ProductService().GetAllProducts();
        var (idWidth, nameWidth, durabilityWidth, availableWidth, priceWidth) = CalculateColumnWidths(products);
        foreach (Product product in products)
        {
            PrintProductLine(product, idWidth, nameWidth, durabilityWidth, availableWidth, priceWidth);
        }
    }

    /* private void ShowShoppingMenu()
    {
        List<Product> products = _marketService.ProductService().GetAllProducts();
        var (idWidth, nameWidth, durabilityWidth, availableWidth, priceWidth) = CalculateColumnWidths(products);
        SetColor(ConsoleColor.Yellow);
        ShowMessage("Verfügbare Produkte:");
        SetColor(ConsoleColor.Green);
        ShowMessage(
        $"{"ID".PadRight(idWidth)} " +
        $"{"Name".PadRight(nameWidth)} " +
        $"{"Haltbarkeit".PadRight(durabilityWidth)} " +
        $"{"Verfügbar".PadRight(availableWidth)} " +
        $"{"Preis".PadRight(priceWidth)}"
        );
        SetColor(ConsoleColor.White);
        ShowAllProducts();
        Console.ResetColor();
    } */

    private void ShowShoppingMenu()
    {
        List<Product> products = _marketService.ProductService().GetAllProducts();
        var (idWidth, nameWidth, durabilityWidth, availableWidth, priceWidth) = CalculateColumnWidths(products);
        var table = new Table();
        table.AddColumn(new TableColumn("[yellow]ID[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Name[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Haltbarkeit[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Verfügbar[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Preis[/]").Centered());
        foreach (Product product in products)
        {
            table.AddRow(
                product.Id.ToString().PadRight(idWidth),
                product.Name.PadRight(nameWidth),
                $"{product.Durability} Tage".PadRight(durabilityWidth),
                $"Verfügbar: {product.AvailableQuantity}".PadRight(availableWidth),
                $"${product.PurchasePrice}/Stück".PadRight(priceWidth)
            );
        }
        table.Border(TableBorder.Rounded);
        table.BorderColor(Color.Green);
        table.Title("[green]Verfügbare Produkte:[/]");
        AnsiConsole.Write(table);
        Console.WriteLine("\nDrücken Sie eine beliebige Taste, um fortzufahren...");
        Console.ReadKey();
    }


    private void ShowSellingMenu(Middleman middleman)
    {
        List<Product> products = _marketService.MiddlemanService().GetOwnedProducts(middleman);
        var (idWidth, nameWidth, quantityWidth, priceWidth) = CalculateSellingColumnWidths(products);
        SetColor(ConsoleColor.Yellow);
        ShowMessage("Produkte zum Verkauf:");
        SetColor(ConsoleColor.Green);
        ShowMessage(
            $"{"ID".PadRight(idWidth)}" +
            $"{"Name".PadRight(nameWidth)}" +
            $"{"Menge".PadRight(quantityWidth)}" +
            $"{"Verkaufspreis".PadRight(priceWidth)}"
        );
        SetColor(ConsoleColor.White);
        ShowOwnedProducts(middleman, idWidth, nameWidth, quantityWidth, priceWidth); // Show owned products with new method
        Console.ResetColor();
    }

    private void ShowOwnedProducts(Middleman middleman, int idWidth, int nameWidth, int quantityWidth, int priceWidth)
    {
        int index = 1;
        foreach (var entry in middleman.Warehouse)
        {
            string id = index.ToString().PadRight(idWidth);
            string name = entry.Key.Name.PadRight(nameWidth);
            string quantity = entry.Value.ToString().PadRight(quantityWidth);
            string sellingPrice = $"${entry.Key.SellingPrice}/Stück".PadRight(priceWidth);
            Console.WriteLine($"{id}{name}{quantity}{sellingPrice}");
            index++;
        }
    }

    private void ShowErrorLog(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Fehler: " + message);
        Console.ResetColor();
    }

    private void ShowMessage(string message)
    {
        Console.WriteLine(message);
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

    private void PrintProductLine(Product product, int idWidth, int nameWidth, int durabilityWidth, int availableWidth, int priceWidth)
    {
        Console.WriteLine(
            $"{product.Id.ToString().PadRight(idWidth)} " +
            $"{product.Name.PadRight(nameWidth)} " +
            $"{($"{product.Durability} Tage").PadRight(durabilityWidth)} " +
            $"{("Verfügbar: " + product.AvailableQuantity.ToString()).PadRight(availableWidth)} " +
            $"{("$" + product.PurchasePrice.ToString() + "/Stück").PadRight(priceWidth)}");
    }

    private void ShowCurrentDay(int currentDay)
    {
        string dayText = $"Tag {currentDay}";
        int padding = 4;
        int frameWidth = dayText.Length + (padding * 2);
        char horizontalLine = '\u2550';     // '═' Double horizontal
        char verticalLine = '\u2551';       // '║' Double vertical
        char topLeftCorner = '\u2554';      // '╔' Double down and right
        char topRightCorner = '\u2557';     // '╗' Double down and left
        char bottomLeftCorner = '\u255A';   // '╚' Double up and right
        char bottomRightCorner = '\u255D';  // '╝' Double up and left
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(topLeftCorner + new string(horizontalLine, frameWidth) + topRightCorner);
        Console.WriteLine(verticalLine + new string(' ', padding) + dayText + new string(' ', padding) + verticalLine);
        Console.WriteLine(bottomLeftCorner + new string(horizontalLine, frameWidth) + bottomRightCorner);
        Console.ResetColor();
    }

    private void SetColor(ConsoleColor color)
    {
        Console.ForegroundColor = color;
    }

    private String GetUserInput()
    {
        return Console.ReadLine() ?? "";
    }

    private string AskUserForInput(string prompt)
    {
        ShowMessage(prompt);
        return GetUserInput() ?? "";
    }
}