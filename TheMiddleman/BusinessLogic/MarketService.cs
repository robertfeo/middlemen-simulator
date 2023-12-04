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
        _bankruptMiddlemen = new List<Middleman>();
        _middlemen = new List<Middleman>();
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
        if (_middlemanService == null || _productService == null)
        {
            throw new Exception("MiddlemanService oder ProductService ist null");
        }
        _middlemen = _middlemanService.RetrieveAllMiddlemen();
        _bankruptMiddlemen = _middlemanService.RetrieveBankruptMiddlemen();
        _productService.CreateProducts();
        SetSimulationDuration(ConsoleUI.RequestSimulationDuration());
        _OnStartOfGame.Invoke();
        while (_currentDay <= _simulationDuration && _middlemen.Count > 0)
        {
            ConsoleUI.ShowCurrentDay(_currentDay);
            SimulateDay();
        }
    }

    public void SimulateDay()
    {
        if (_currentDay > 1) { _productService.UpdateProducts(); }
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
        if (!ValidateSelectedProductForSelling(userInput, middleman, out Product? selectedProduct)) { return; }
        if (selectedProduct == null)
        {
            ConsoleUI.ShowErrorLog("Es wurde kein Produkt ausgewählt.\n");
            return;
        }
        string quantityInput = AskQuantity($"Wieviel von {selectedProduct.Name} verkaufen?");
        if (!ValidateQuantityToSell(middleman, quantityInput, selectedProduct, out int quantityToSell)) { return; }
        _middlemanService.SellProduct(middleman, selectedProduct, quantityToSell);
        ConsoleUI.ShowMessage($"Sie haben {quantityToSell}x {selectedProduct.Name} verkauft.");
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
        if (_currentDay > _simulationDuration || _middlemen.Count == 0) { EndSimulation(); }
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