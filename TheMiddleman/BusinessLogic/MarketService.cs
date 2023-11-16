using TheMiddleman.Entity;

public class MarketService
{
    private readonly ProductService _productService;
    private readonly MiddlemanService _middlemanService;
    public Action<Middleman, int> _OnDayStart { get; set; } = delegate { };
    public Action<int> _OnDayChange { get; set; } = delegate { };
    public Action<Middleman> _OnBankruptcy { get; set; } = delegate { };
    public Action<List<Middleman>> _OnEndOfGame { get; set; } = delegate { };
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

    public void InitiatePurchase(Middleman middleman, int productId, int quantity)
    {
        // Business logic for purchasing products...
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
