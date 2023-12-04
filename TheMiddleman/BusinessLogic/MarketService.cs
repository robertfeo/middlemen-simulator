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

    public MarketService()
    {
        _productService = new ProductService();
        _middlemanService = new MiddlemanService();
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
        _middlemen = _middlemanService.RetrieveMiddlemen();
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
        List<Middleman> bankruptMiddlemen = ProcessMiddlemenEachDay();
        SaveBankruptMiddlemen(bankruptMiddlemen);
        ChangeMiddlemanOrder();
        CheckForEndOfSimulation();
    }

    private List<Middleman> ProcessMiddlemenEachDay()
    {
        List<Middleman> bankruptMiddlemen = new List<Middleman>();
        foreach (var middleman in _middlemen)
        {
            try
            {
                if (middleman.AccountBalance <= 0)
                {
                    throw new InsufficientFundsException("Nicht genügend Geld für die Lagerkosten vorhanden.");
                }
                _middlemanService.DeductStorageCosts(middleman);
                _OnDayStart.Invoke(middleman, _currentDay);
            }
            catch (InsufficientFundsException ex)
            {
                _OnBankruptcy.Invoke(middleman, ex);
                bankruptMiddlemen.Add(middleman);
            }
        }
        return bankruptMiddlemen;
    }

    private void SaveBankruptMiddlemen(List<Middleman> bankruptMiddlemen)
    {
        foreach (var bankruptMiddleman in bankruptMiddlemen)
        {
            _middlemanService.AddBankruptMiddleman(bankruptMiddleman);
            _middlemen.Remove(bankruptMiddleman);
        }
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