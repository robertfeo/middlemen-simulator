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
        var rule = new Rule("[darkolivegreen2]:gear:  Konfiguration der Simulation[/]");
        rule.LeftJustified();
        AnsiConsole.Write(rule);
        RequestSimulationDuration();
        ShowCreationMiddlemen();
        _marketService.RunSimulation();
    }

    private void InitializeConsoleUI()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        AnsiConsole.Write(new FigletText("The Middleman").LeftJustified().Color(Color.Red));
        BindFunctionsFromOutsideClass();
    }

    private void BindFunctionsFromOutsideClass()
    {
        _marketService._OnDayStart += ShowMenuAndTakeAction;
        _marketService._OnDayChange += ShowCurrentDay;
        _marketService._OnBankruptcy += ShowMiddlemanBankroped;
        _marketService._OnEndOfGame += ShowEndOfGame;
    }

    private void ShowEndOfGame(List<Middleman> middlemen)
    {
        if (middlemen.Count == 0)
        {
            ShowBankruptMiddlemen();
        }
        else
        {
            ShowRankingMiddlemen(middlemen);
        }
    }

    private void ShowBankruptMiddlemen()
    {
        Panel panel = new Panel("[red]Alle Zwischenhändler sind bankrott gegangen.[/]");
        panel.Header("[yellow]Ende der Simulation[/]");
        AnsiConsole.Write(panel);
    }

    private void ShowRankingMiddlemen(List<Middleman> middlemen)
    {
        Table table = CreateRankingTable();
        int rank = 1;
        foreach (Middleman middleman in middlemen)
        {
            if (rank == 1)
            {
                table.AddRow($"[bold]{rank}:crown:[/]", $"[bold]{middleman.Name}[/]", $"[bold]{CurrencyFormatter.FormatPrice(middleman.AccountBalance)}[/]");
                rank++;
                continue;
            }
            table.AddRow(rank.ToString(), middleman.Name!, CurrencyFormatter.FormatPrice(middleman.AccountBalance));
            rank++;
        }
        Panel panel = new Panel(table);
        panel.RoundedBorder();
        panel.Header("[yellow]Ende der Simulation[/]");
        AnsiConsole.Write(panel);
    }

    private Table CreateRankingTable()
    {
        Table table = new Table().BorderColor(Color.Green);
        table.RoundedBorder();
        table.Title("[green]Rangliste[/]");
        table.AddColumn(new TableColumn("[u]Platz[/]"));
        table.AddColumn(new TableColumn("[u]Name[/]"));
        table.AddColumn(new TableColumn("[u]Kontostand[/]"));
        return table;
    }

    private void ShowMiddlemanBankroped(Middleman middleman)
    {
        string message = $"[rapidblink][white]{middleman.Name} ist pleite und wird ausgeschlossen.[/][/]";
        Panel panel = new Panel(new Markup(message))
            .Header("[indianred1]:bell: Information[/]");
        AnsiConsole.Write(panel);
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
            new TextPrompt<int>("Wie viele Zwischenhändler nehmen teil? (Anzahl):")
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
                .AddChoices(["Einfach", "Normal", "Schwer"])
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

    private void DisplayMiddlemanInfo(Middleman middleman, int currentDay)
    {
        var panel = new Panel(
            new Markup(
                $"[bold]Kontostand:[/] {CurrencyFormatter.FormatPrice(middleman.AccountBalance)}\n" +
                $"[bold]Lagerkapazität:[/] {middleman.Warehouse.Values.Sum()}/{middleman.MaxStorageCapacity}\n" +
                $"[bold]Tag:[/] {currentDay}"
            ))
            .Header($" {middleman.Name} von {middleman.Company} ")
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
            AddColumnsDailyReport(ref table);
            AddRowsDailyRepot(ref table, middleman);
            table.Border(TableBorder.Rounded);
            table.BorderColor(Color.Green);
            table.Title($"Tagesbericht für {middleman.Name}");
            AnsiConsole.Write(table);
            Console.WriteLine("Drücken Sie Enter, um fortzufahren...");
            Console.ReadLine();
            _dailyReportShown = true;
            return;
        }
    }

    private void AddColumnsDailyReport(ref Table table)
    {
        table.AddColumn(new TableColumn("Kategorie").LeftAligned());
        table.AddColumn(new TableColumn("Betrag").LeftAligned());
    }

    private void AddRowsDailyRepot(ref Table table, Middleman middleman)
    {
        table.AddRow("Kontostand zu Beginn des letzten Tages", $"{CurrencyFormatter.FormatPrice(middleman.PreviousDayBalance)}");
        table.AddRow("Ausgaben für Einkäufe", $"{CurrencyFormatter.FormatPrice(middleman.DailyExpenses)}");
        table.AddRow("Einnahmen aus Verkäufen", $"{CurrencyFormatter.FormatPrice(middleman.DailyEarnings)}");
        table.AddRow("Lagerkosten", $"{CurrencyFormatter.FormatPrice(middleman.DailyStorageCosts)}");
        table.AddRow("Aktueller Kontostand zu Beginn des Tages", $"{CurrencyFormatter.FormatPrice(middleman.AccountBalance)}");
        if (middleman.CurrentLoan != null)
        {
            table.AddRow("Kredit", $"{CurrencyFormatter.FormatPrice(middleman.CurrentLoan.Amount)}");
            table.AddRow("Kreditfälligkeit", $"{middleman.CurrentLoan.DueDay - _marketService.GetCurrentDay()} Tage");
        }
        if (middleman.CurrentLoan == null && middleman.LoanRepaymentNotified)
        {
            table.AddRow("[green]💸 Kredit wurde zurückgezahlt.[/]");
            middleman.LoanRepaymentNotified = false;
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
            Console.WriteLine("k) Kredit aufnehmen");
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
            case "k":
                ShowLoan(middleman);
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

    private void ShowLoan(Middleman middleman)
    {
        var loanOptions = new Dictionary<int, (double amount, double interestRate, double repaymentAmount)>
            {
                {1, (5000, 0.03, 5150)},
                {2, (10000, 0.05, 10500)},
                {3, (25000, 0.08, 27000)}
            };
        var prompt = new SelectionPrompt<string>()
            .Title("Wählen Sie einen Kredit aus:")
            .PageSize(10)
            .AddChoices(new[] {
                "1) $5000 mit 3% Zinsen (=$5150 Rückzahlung)",
                "2) $10000 mit 5% Zinsen (=$10500 Rückzahlung)",
                "3) $25000 mit 8% Zinsen (=$27000 Rückzahlung)"
            });
        string choice = AnsiConsole.Prompt(prompt);
        int selectedOption = int.Parse(choice.Substring(0, 1));
        (double amount, double interestRate, double repaymentAmount) = loanOptions[selectedOption];
        try
        {
            _marketService.MiddlemanService().RegisterNewLoan(_marketService.GetCurrentDay(), middleman, amount, interestRate);
            ShowMessage($"Kredit über ${amount} mit {interestRate * 100}% Zinsen erfolgreich aufgenommen. Rückzahlung: ${repaymentAmount}.");
        }
        catch (LoanAlreadyExistsException ex)
        {
            ShowErrorLog(ex.Message);
        }
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
            Panel panel = new Panel(new Markup($"Sie haben {quantity}x {selectedProduct.Name} gekauft."))
            .Header("[indianred1]:bell: Information[/]");
            AnsiConsole.Write(panel);
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
            Product selectedProduct = _marketService.MiddlemanService().GetProductByID(Int16.Parse(userInput));
            string quantityInput = AskUserForInput($"Wie viele Einheiten von {selectedProduct.Name} möchten Sie verkaufen?");
            int quantityToSell = int.Parse(quantityInput);
            _marketService.MiddlemanService().SellProduct(middleman, selectedProduct, quantityToSell);
            Panel panel = new Panel(new Markup($"Erfolgreich {quantityToSell}x {selectedProduct.Name} verkauft."))
            .Header("[indianred1]:bell: Information[/]");
            AnsiConsole.Write(panel);
        }
        catch (FormatException) { ShowErrorLog("Ungültige Eingabe."); }
        catch (ProductNotAvailableException ex) { ShowErrorLog(ex.Message); }
        catch (InvalidOperationException) { ShowErrorLog("Ungültige Eingabe."); }
        catch (ArgumentOutOfRangeException) { ShowErrorLog("Produkt ID nicht gefunden"); }
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

    private void ShowProductBreakdownChart(Middleman middleman)
    {
        AnsiConsole.Write("Verteilung der Produkte im Lager:\n");
        double totalCapacity = middleman.MaxStorageCapacity;
        double usedCapacity = middleman.Warehouse.Sum(item => item.Value);
        double emptyCapacity = totalCapacity - usedCapacity;
        double emptyCapacityPercentage = (emptyCapacity / totalCapacity) * 100;
        var chart = new BreakdownChart()
            .Width(60)
            .FullSize()
            .ShowPercentage();
        foreach (var entry in middleman.Warehouse)
        {
            Product product = entry.Key;
            int quantity = entry.Value;
            double percentage = (quantity / totalCapacity) * 100;
            Color itemColor = GetColorForProduct(product);
            chart.AddItem(product.Name, percentage, itemColor);
        }
        if (emptyCapacityPercentage > 0)
        {
            chart.AddItem("Leer", emptyCapacityPercentage, Color.Grey);
        }
        AnsiConsole.Write(chart);
    }

    private static Color GetColorForProduct(Product product)
    {
        Dictionary<string, Color> colorMappings = new Dictionary<string, Color>()
    {
        { "Gurke", Color.Green },
        { "Aprikose", Color.Orange1 },
        { "Tomate", Color.Red3 },
        { "Kartoffel", Color.RosyBrown },
        { "Zwiebel", Color.Olive },
        { "Karotte", Color.DarkOrange },
        { "Apfel", Color.Green3 },
        { "Banane", Color.Yellow },
        { "Orangensaft", Color.Orange3 },
        { "Milch", Color.NavajoWhite1 }
    };
        if (colorMappings.ContainsKey(product.Name))
        {
            return colorMappings[product.Name];
        }
        return Color.Grey;
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
        GenerateProductsForPurchaseTable(ref table);
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

    private void GenerateProductsForPurchaseTable(ref Table table)
    {
        table.AddColumn(new TableColumn("[yellow]ID[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Name[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Haltbarkeit[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Verfügbar[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Preis[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Rabatt[/]").Centered());
    }

    private void ShowSellingMenu(Middleman middleman)
    {
        int index = 1;
        var table = new Table();
        GenerateProductsForSaleTable(ref table);
        foreach (var entry in middleman.Warehouse)
        {
            Product product = entry.Key;
            int quantity = entry.Value;
            table.AddRow(
                index.ToString(),
                product.Name,
                quantity.ToString(),
                $"{CurrencyFormatter.FormatPrice(product.SellingPrice)} / Stück"
            );
            index++;
        }
        table.Border(TableBorder.Rounded);
        table.BorderColor(Color.Green);
        table.Title("[green]Produkte zum Verkauf:[/]");
        table.Collapse();
        AnsiConsole.Write(table);
        ShowProductBreakdownChart(middleman);
        ShowMessage("\nz) Zurück");
    }

    private void GenerateProductsForSaleTable(ref Table table)
    {
        table.AddColumn(new TableColumn("[yellow]ID[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Name[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Menge[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Verkaufspreis[/]").Centered());
    }

    private void ShowErrorLog(string message)
    {
        AnsiConsole.MarkupLine($"[red]Fehler:[/] {message}");
    }

    private void ShowMessage(string message)
    {
        AnsiConsole.WriteLine(message);
    }

    private void ShowCurrentDay(int currentDay)
    {
        var rule = new Rule($"[lime]Tag {currentDay}[/]");
        rule.LeftJustified();
        AnsiConsole.Write(rule);
        Console.WriteLine("\n");
    }

    private string GetUserInput()
    {
        return Console.ReadLine() ?? "";
    }

    private string AskUserForInput(string prompt)
    {
        ShowMessage(prompt);
        return GetUserInput() ?? "";
    }
}