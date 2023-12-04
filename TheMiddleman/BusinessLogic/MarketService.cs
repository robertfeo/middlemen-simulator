using TheMiddleman.Entity;

public class MarketService
{
    private readonly ProductService _productService;
    private readonly MiddlemanService _middlemanService;
    public Action<Middleman, int> _OnDayStart { get; set; } = delegate { };
    public Action<int> _OnDayChange { get; set; } = delegate { };
    public Action<Middleman, InsufficientFundsException> _OnBankruptcy { get; set; } = delegate { };
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
        _middlemen = _middlemanService.RetrieveAllMiddlemen();
        _productService.CreateProducts();
        _OnStartOfGame?.Invoke();
        for (_currentDay = 1; _currentDay <= _simulationDuration && _middlemen.Any(); _currentDay++)
        {
            _OnDayChange?.Invoke(_currentDay);
            SimulateDay();
            if (_middlemen.Count == 0)
            {
                break;
            }
        }
        EndSimulation();
    }

    public void SimulateDay()
    {
        if (_currentDay > 1) { _productService.UpdateProducts(); }
        foreach (var middleman in _middlemen)
        {
            try
            {
                _middlemanService.DeductStorageCosts(middleman);
            }
            catch (InsufficientFundsException ex)
            {
                _OnBankruptcy?.Invoke(middleman, ex);
                continue;
            }
            _OnDayStart.Invoke(middleman, _currentDay);
        }
        foreach (var bankruptMiddleman in _bankruptMiddlemen)
        {
            _middlemen.Remove(bankruptMiddleman);
        }
        ChangeMiddlemanOrder();
        CheckForEndOfSimulation();
    }

    public void InitiateSelling(Middleman middleman, string userInput)
    {
        if (!ValidateSelectedProductForSelling(userInput, middleman, out Product? selectedProduct)) { return; }
        if (selectedProduct == null)
        {
            throw new UserInputException("Selected product is null.");
        }
        string quantityInput = AskQuantity($"Wieviel von {selectedProduct.Name} verkaufen?");
        if (!ValidateQuantityToSell(middleman, quantityInput, selectedProduct, out int quantityToSell)) { return; }
        _middlemanService.SellProduct(middleman, selectedProduct, quantityToSell);
    }

    private bool ValidateQuantityToSell(Middleman middleman, string quantityToSellInput, Product selectedProduct, out int quantityToSell)
    {
        if (!int.TryParse(quantityToSellInput, out quantityToSell) || quantityToSell <= 0)
        {
            throw new UserInputException("Ungültige Menge. Bitte erneut versuchen.");
        }
        var productInWarehouse = middleman.Warehouse.FirstOrDefault(p => p.Key.Id == selectedProduct.Id).Key;
        if (productInWarehouse == null || quantityToSell > middleman.Warehouse[productInWarehouse])
        {
            throw new ProductNotAvailableException("Nicht genügend Produkte verfügbar. Bitte erneut versuchen.");
        }
        return true;
    }

    private bool ValidateSelectedProductForSelling(string userSelectedProductId, Middleman middleman, out Product? selectedProduct)
    {
        selectedProduct = null;
        if (!int.TryParse(userSelectedProductId, out int selectedProductId) || selectedProductId <= 0)
        {
            throw new UserInputException("Ungültige Eingabe.");
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
            throw new ProductNotAvailableException("Dieses Produkt ist nicht in Ihrem Inventar.");
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
        if (_currentDay > _simulationDuration || _middlemen.Count == 0)
        {
            EndSimulation();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetSimulationDuration(int duration)
    {
        _simulationDuration = duration;
    }
}