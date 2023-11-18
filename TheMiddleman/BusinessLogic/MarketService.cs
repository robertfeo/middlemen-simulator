using TheMiddleman.Entity;

public class MarketService
{
    private readonly ProductService _productService;
    private readonly MiddlemanService _middlemanService;
    public Action<Middleman, int> _OnDayStart { get; set; } = delegate { };
    public Action<int> _OnDayChange { get; set; } = delegate { };
    public Action<Middleman> _OnBankruptcy { get; set; } = delegate { };
    public Action<List<Middleman>> _OnEndOfGame { get; set; } = delegate { };
    public Action _OnStartOfGame { get; set; } = delegate { };
    public int _currentDay = 1;
    private int _simulationDuration;
    private List<Middleman> _middlemen;
    private List<Middleman> _bankruptMiddlemen;

    public MarketService()
    {
        _productService = new ProductService();
        _middlemanService = new MiddlemanService();
        _middlemen = _middlemanService.RetrieveAllMiddlemen();
        _bankruptMiddlemen = _middlemanService.RetrieveBankruptMiddlemen();
    }

    public MiddlemanService MiddlemanService()
    {
        return _middlemanService;
    }

    public ProductService ProductService()
    {
        return _productService;
    }

    public void RunSimulation()
    {
        SetSimulationDuration(ConsoleUI.RequestSimulationDuration());
        _OnStartOfGame.Invoke();
        while (_currentDay <= _simulationDuration && _middlemen.Count > 0)
        {
            ConsoleUI.PrintDayInFrame(_currentDay);
            SimulateDay();
        }
    }

    public void SimulateDay()
    {
        if (_currentDay > 1)
        {
            _productService.UpdateProducts();
        }
        foreach (var middleman in _middlemen)
        {
            _middlemanService.DeductStorageCosts(middleman);
            if (middleman.AccountBalance < 0)
            {
                _OnBankruptcy.Invoke(middleman);
                _bankruptMiddlemen.Add(middleman);
                continue;
            }
            _OnDayStart.Invoke(middleman, _currentDay);
        }
        foreach (var bankruptMiddleman in _bankruptMiddlemen)
        {
            _middlemen.Remove(bankruptMiddleman);
        }
        ChangeMiddlemanOrder();
        _currentDay++;
        CheckForEndOfSimulation();
    }

    public void InitiateSelling(Middleman middleman, string userInput)
    {
        if (!ValidateSelectedProductForSelling(userInput, middleman, out Product? selectedProduct))
        {
            return;
        }
        if (selectedProduct == null)
        {
            ConsoleUI.ShowErrorLog("Es wurde kein Produkt ausgewählt.\n");
            return;
        }
        string quantityInput = AskQuantity($"Wieviel von {selectedProduct.Name} verkaufen?");
        if (!ValidateQuantityToSell(middleman, quantityInput, selectedProduct, out int quantityToSell))
        {
            return;
        }
        _middlemanService.Sale(middleman, selectedProduct, quantityToSell);
        ConsoleUI.ShowMessage($"Sie haben {quantityToSell}x {selectedProduct.Name} verkauft.");
    }

    public void InitiatePurchase(Middleman middleman, string userInput)
    {
        if (!ValidateSelectedProduct(userInput, out Product? selectedProduct))
        {
            return;
        }
        if (selectedProduct == null)
        {
            ConsoleUI.ShowErrorLog("Es wurde kein Produkt ausgewählt.\n");
            return;
        }
        string quantityInput = AskQuantity($"Wieviel von {selectedProduct.Name} kaufen?");
        if (!ValidateQuantityToBuy(quantityInput, selectedProduct, out int quantityToBuy))
        {
            return;
        }
        _middlemanService.Purchase(middleman, selectedProduct, quantityToBuy, out string errorLog);
        if (!string.IsNullOrEmpty(errorLog))
        {
            ConsoleUI.ShowErrorLog(errorLog + "\n");
            return;
        }
        ConsoleUI.ShowMessage($"Sie haben {quantityToBuy}x {selectedProduct.Name} gekauft.");
    }

    private bool ValidateQuantityToBuy(string quantityToBuyInput, Product selectedProduct, out int quantityToBuy)
    {
        if (!int.TryParse(quantityToBuyInput, out quantityToBuy) || quantityToBuy <= 0)
        {
            ConsoleUI.ShowErrorLog("Ungültige Menge. Bitte erneut versuchen.\n");
            return false;
        }
        if (quantityToBuy > selectedProduct.AvailableQuantity)
        {
            ConsoleUI.ShowErrorLog("Nicht genügend Produkte verfügbar. Bitte erneut versuchen.\n");
            return false;
        }
        return true;
    }

    private bool ValidateQuantityToSell(Middleman middleman, string quantityToSellInput, Product selectedProduct, out int quantityToSell)
    {
        if (!int.TryParse(quantityToSellInput, out quantityToSell) || quantityToSell <= 0)
        {
            ConsoleUI.ShowErrorLog("Ungültige Menge. Bitte erneut versuchen.\n");
            return false;
        }
        var productInWarehouse = middleman.Warehouse.FirstOrDefault(p => p.Key.Id == selectedProduct.Id).Key;
        if (productInWarehouse == null || quantityToSell > middleman.Warehouse[productInWarehouse])
        {
            ConsoleUI.ShowErrorLog("Nicht genügend Produkte verfügbar. Bitte erneut versuchen.\n");
            return false;
        }
        return true;
    }

    private bool ValidateSelectedProduct(string userSelectedProductId, out Product? selectedProduct)
    {
        if (!int.TryParse(userSelectedProductId, out int selectedProductId) || selectedProductId <= 0)
        {
            ConsoleUI.ShowMessage("Ungültige Eingabe!");
            selectedProduct = null;
            return false;
        }
        selectedProduct = _productService.FindProductById(selectedProductId)!;
        if (selectedProduct == null || selectedProduct.AvailableQuantity == 0)
        {
            ConsoleUI.ShowErrorLog("Dieses Produkt ist nicht mehr verfügbar. Bitte erneut versuchen.\n");
            return false;
        }
        return true;
    }

    private bool ValidateSelectedProductForSelling(string userSelectedProductId, Middleman middleman, out Product? selectedProduct)
    {
        selectedProduct = null;
        if (!int.TryParse(userSelectedProductId, out int selectedProductId) || selectedProductId <= 0)
        {
            ConsoleUI.ShowErrorLog("Ungültige Eingabe!");
            return false;
        }
        int index = selectedProductId - 1;
        if (index >= 0 && index < middleman.Warehouse.Count)
        {
            var entry = middleman.Warehouse.ElementAt(index);
            selectedProduct = entry.Key;
            return true;
        }
        else
        {
            ConsoleUI.ShowErrorLog("Dieses Produkt ist nicht in Ihrem Inventar.\n");
            return false;
        }
    }

    private string AskQuantity(string prompt)
    {
        ConsoleUI.ShowMessage(prompt);
        return ConsoleUI.GetUserInput() ?? "";
    }

    private void CheckForEndOfSimulation()
    {
        if (_currentDay > _simulationDuration || _middlemen.Count == 0)
        {
            EndSimulation();
        }
    }

    private void EndSimulation()
    {
        if (_middlemen.Count > 0)
        {
            _middlemen.Sort((x, y) => y.AccountBalance.CompareTo(x.AccountBalance));
        }
        _OnEndOfGame.Invoke(_middlemen);
        Environment.Exit(0);
    }

    private void ChangeMiddlemanOrder()
    {
        if (_middlemen.Count > 1)
        {
            var firstMiddleman = _middlemen[0];
            _middlemen.RemoveAt(0);
            _middlemen.Add(firstMiddleman);
        }
    }

    public bool CheckIfMiddlemanIsLastBankroped(Middleman middleman)
    {
        return _middlemen.Count == 1 && _middlemen[0] == middleman;
    }

    public void SetSimulationDuration(int duration)
    {
        _simulationDuration = duration;
    }
}
