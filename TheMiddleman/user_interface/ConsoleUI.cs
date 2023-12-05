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
            var rule = new Rule("[lime]Drücken Sie eine beliebige Taste, um fortzufahren...[/]");
            rule.LeftJustified();
            AnsiConsole.Write(rule);
        }
        catch (Exception)
        {
            Console.WriteLine("Willkommen bei The Middleman!");
            Console.WriteLine("Drücken Sie eine beliebige Taste, um fortzufahren...");
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
        int priceWidth = products.Max(p => $"{CurrencyFormatter.FormatPrice(p.PurchasePrice)} / Stück".Length) + 4;
        return (idWidth, nameWidth, durabilityWidth, availableWidth, priceWidth);
    }

    private (int idWidth, int nameWidth, int quantityWidth, int priceWidth) CalculateSellingColumnWidths(List<Product> products)
    {
        int idWidth = products.Max(p => p.Id.ToString().Length) + 4;
        int nameWidth = products.Max(p => p.Name.Length) + 4;
        int quantityWidth = "Menge".Length + 4;
        int priceWidth = products.Max(p => $"{p.SellingPrice}/Stück".Length) + 4;
        return (idWidth, nameWidth, quantityWidth, priceWidth);
    }

    private void ShowEndOfGame(List<Middleman> middlemen)
    {
        Panel? panel;
        if (middlemen.Count == 0)
        {
            panel = new Panel("[red]Alle Zwischenhändler sind bankrott gegangen[/]");
            panel.Header("[yellow]Ende der Simulation[/]");
        }
        else
        {
            panel = new Panel("[green]Folgende Zwischenhändler haben überlebt:[/]");
            Table table = new Table().BorderColor(Color.Green);
            table.AddColumn(new TableColumn("[u]Platz[/]"));
            table.AddColumn(new TableColumn("[u]Name[/]"));
            table.AddColumn(new TableColumn("[u]Kontostand[/]"));
            int rank = 1;
            foreach (Middleman middleman in middlemen)
            {
                table.AddRow(rank.ToString(), middleman.Name!, CurrencyFormatter.FormatPrice(middleman.AccountBalance));
                rank++;
            }
            panel = new Panel(table);
            panel.Header("[yellow]Ende der Simulation[/]");
        }
        AnsiConsole.Write(panel);
    }

    private void ShowMiddlemanBankroped(Middleman middleman)
    {
        if (_marketService.CheckIfMiddlemanIsLastBankrupted(middleman))
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
        var duration = AnsiConsole.Prompt(
            new TextPrompt<int>("Wie lange soll die Simulation laufen? (Anzahl der Tage):")
                .Validate(days => days > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Bitte geben Sie eine positive Zahl ein.[/]"))
        );
        _marketService.SetSimulationDuration(duration);
    }

    private int RequestAmountOfMiddlemen()
    {
        int amount = AnsiConsole.Prompt(
            new TextPrompt<int>("Wieviel Zwischenhändler nehmen teil? (Anzahl):")
                .Validate(numberMiddlemen => numberMiddlemen > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Ungültige Eingabe. Bitte geben Sie eine positive Zahl ein.[/]"))
        );
        return amount;
    }

    private string AskForMiddlemanName(int index)
    {
        return AnsiConsole.Ask<string>($"Name von Zwischenhändler {index}:");
    }

    private string InquireCompany(string name)
    {
        return AnsiConsole.Ask<string>($"Name der Firma von {name}:");
    }

    private int DetermineInitialBalance()
    {
        var balanceOptions = new Dictionary<string, int>
        {
            ["Einfach"] = 15000,
            ["Normal"] = 10000,
            ["Schwer"] = 7000
        };

        var difficulty = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Schwierigkeitsgrad auswählen:")
                .PageSize(10)
                .AddChoices(new[] { "Einfach", "Normal", "Schwer" })
        );

        return balanceOptions[difficulty];
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

    /* private void DisplayMiddlemanInfo(Middleman middleman, int currentDay)
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
        ShowMessage($"{verticalLine} Kontostand: {CurrencyFormatter.FormatPrice(middleman.AccountBalance)}".PadRight(43) + verticalLine);
        ShowMessage($"{verticalLine} Lagerkapazität: {middleman.Warehouse.Values.Sum()}/{middleman.MaxStorageCapacity}".PadRight(43) + verticalLine);
        ShowMessage($"{verticalLine} Tag: {currentDay}".PadRight(43) + verticalLine);
        Console.ForegroundColor = ConsoleColor.Cyan;
        ShowMessage(bottomBorder);
        Console.ResetColor();
    } */

    private void DisplayMiddlemanInfo(Middleman middleman, int currentDay)
    {
        var panel = new Panel(
            new Markup(
                $"[bold]Kontostand:[/] {CurrencyFormatter.FormatPrice(middleman.AccountBalance)}\n" +
                $"[bold]Lagerkapazität:[/] {middleman.Warehouse.Values.Sum()}/{middleman.MaxStorageCapacity}\n" +
                $"[bold]Tag:[/] {currentDay}"
            ))
            .Header($"{middleman.Name} von {middleman.Company}")
            .Border(BoxBorder.Rounded)
            .BorderStyle(new Style(Color.Cyan1));
        AnsiConsole.Write(panel);
    }

    private void ShowDailyReport(Middleman middleman)
    {
        if (_dailyReportShown) { return; }
        else
        {
            var table = new Table();
            table.AddColumn(new TableColumn("Kategorie").LeftAligned());
            table.AddColumn(new TableColumn("Betrag").LeftAligned());
            table.AddRow("Kontostand zu Beginn des letzten Tages", $"{CurrencyFormatter.FormatPrice(middleman.PreviousDayBalance)}");
            table.AddRow("Ausgaben für Einkäufe", $"{CurrencyFormatter.FormatPrice(middleman.DailyExpenses)}");
            table.AddRow("Einnahmen aus Verkäufen", $"{CurrencyFormatter.FormatPrice(middleman.DailyEarnings)}");
            table.AddRow("Lagerkosten", $"{CurrencyFormatter.FormatPrice(middleman.DailyStorageCosts)}");
            table.AddRow("Aktueller Kontostand zu Beginn des Tages", $"{CurrencyFormatter.FormatPrice(middleman.AccountBalance)}");
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
        ShowShoppingMenu(middleman);
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
            ShowMessage("Um wie viele Einheiten möchten Sie das Lager erweitern? (50 € pro Einheit)");
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

    private void ShowShoppingMenu(Middleman currentMiddleman)
    {
        List<Product> products = _marketService.ProductService().GetAllProducts();
        var table = new Table();
        table.AddColumn(new TableColumn("[yellow]ID[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Name[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Haltbarkeit[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Verfügbar[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Preis[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Rabatt[/]").Centered()); // Neue Spalte für Rabatte
        foreach (Product product in products)
        {
            double discount = _marketService.MiddlemanService().CalculateDiscount(product, currentMiddleman);
            string discountDisplay = discount > 0 ? $"{discount * 100}%" : "0%";
            table.AddRow(
                product.Id.ToString(),
                product.Name,
                $"{product.Durability} Tage",
                $"Verfügbar: {product.AvailableQuantity}",
                $"{CurrencyFormatter.FormatPrice(product.PurchasePrice)} / Stück",
                discountDisplay
            );
        }
        table.Border(TableBorder.Rounded);
        table.BorderColor(Color.Green);
        table.Title("[green]Verfügbare Produkte:[/]");
        table.Collapse();
        AnsiConsole.Write(table);
    }

    private void ShowSellingMenu(Middleman middleman)
    {
        List<Product> products = _marketService.MiddlemanService().GetOwnedProducts(middleman);
        int index = 1;
        var (idWidth, nameWidth, quantityWidth, priceWidth) = CalculateSellingColumnWidths(products);
        var table = new Table();
        table.AddColumn(new TableColumn("[yellow]ID[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Name[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Menge[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Verkaufspreis[/]").Centered());
        foreach (var entry in middleman.Warehouse)
        {
            Product product = entry.Key;
            int quantity = entry.Value;
            table.AddRow(
                index.ToString().PadRight(idWidth),
                product.Name.PadRight(nameWidth),
                quantity.ToString().PadRight(quantityWidth),
                $"{CurrencyFormatter.FormatPrice(product.SellingPrice)} / Stück".PadRight(priceWidth)
            );
            index++;
        }
        table.Border(TableBorder.Rounded);
        table.BorderColor(Color.Green);
        table.Title("[green]Produkte zum Verkauf:[/]");
        table.Collapse();
        AnsiConsole.Write(table);
    }

    private void ShowErrorLog(string message)
    {
        AnsiConsole.MarkupLine($"[red]Fehler:[/] {message}");
    }

    private void ShowMessage(string message)
    {
        AnsiConsole.WriteLine(message);
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

    private void ShowCurrentDay(int currentDay)
    {
        var rule = new Rule($"[lime]Tag {currentDay}[/]");
        rule.LeftJustified();
        AnsiConsole.Write(rule);
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