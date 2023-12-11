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
        _middlemen = new List<Middleman>();
        _bankruptMiddlemen = new List<Middleman>();
    }

    public MiddlemanService MiddlemanService() { return _middlemanService; }

    public ProductService ProductService() { return _productService; }

    public void RunSimulation()
    {
        _middlemen = _middlemanService.RetrieveMiddlemen();
        _bankruptMiddlemen = _middlemanService.RetrieveBankruptMiddlemen();
        _productService.CreateProducts();
        _OnStartOfGame?.Invoke();
        while (_currentDay <= _simulationDuration && _middlemen.Count > 0)
        {
            _OnDayChange?.Invoke(_currentDay);
            SimulateDay();
            _currentDay++;
        }
    }

    public void SimulateDay()
    {
        if (_currentDay > 1) { _productService.UpdateProducts(); }
        SimulateMiddlemenDay();
    }

    private void SimulateMiddlemenDay()
    {
        foreach (var middleman in _middlemen.ToList())
        {
            try
            {
                if (_middlemanService.VerifyLoanDue(middleman, _currentDay))
                {
                    _middlemanService.HandleLoanRepayment(_currentDay, middleman);
                }
                _middlemanService.DeductStorageCosts(middleman);
                if (middleman.AccountBalance <= 0)
                {
                    _OnBankruptcy.Invoke(middleman);
                    continue;
                }
            }
            catch (InsufficientFundsException)
            {
                _OnBankruptcy.Invoke(middleman);
                _bankruptMiddlemen.Add(middleman);
                continue;
            }
            _OnDayStart.Invoke(middleman, _currentDay);
        }
        SaveBankruptMiddlemen();
        ChangeMiddlemanOrder();
        CheckForEndOfSimulation();
    }

    private void SaveBankruptMiddlemen()
    {
        foreach (var bankruptMiddleman in _bankruptMiddlemen)
        {
            if (!bankruptMiddleman.BankruptcyNotified)
            {
                bankruptMiddleman.BankruptcyNotified = true;
            }
            _middlemen.Remove(bankruptMiddleman);
        }
    }

    private void CheckForEndOfSimulation()
    {
        if (_currentDay >= _simulationDuration || !_middlemen.Any(m => m.AccountBalance >= 0))
        {
            EndSimulation();
        }
    }

    private void EndSimulation()
    {
        _middlemen.Sort((x, y) => y.AccountBalance.CompareTo(x.AccountBalance));
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

    public void SetSimulationDuration(int duration)
    {
        _simulationDuration = duration;
    }

    public bool CheckIfMiddlemanIsLastBankrupted(Middleman middleman)
    {
        return _middlemanService.CheckIfMiddlemanIsLastBankrupted(middleman);
    }

    public int GetCurrentDay()
    {
        return _currentDay;
    }

    public void SetCurrentDay(int day)
    {
        _currentDay = day;
    }
}