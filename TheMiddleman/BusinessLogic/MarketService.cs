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

    private void CheckForEndOfSimulation()
    {
        if (!_middlemen.Any())
        {
            _OnEndOfGame.Invoke(_bankruptMiddlemen);
            Console.WriteLine();
            Environment.Exit(0);
        }
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
}
