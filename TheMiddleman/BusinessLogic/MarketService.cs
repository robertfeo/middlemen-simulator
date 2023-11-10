using TheMiddleman.Entity;

public class MarketService
{
    private readonly ProductService _productService;
    private readonly MiddlemanService _middlemanService;
    public Action<Middleman, int> _OnDayStart { get; set; } = delegate { };
    public Action<int> _OnDayChange { get; set; } = delegate { };
    public int _currentDay = 1;
    private List<Middleman> _middlemen;

    public MarketService()
    {
        _productService = new ProductService();
        _middlemanService = new MiddlemanService();
        _middlemen = _middlemanService.RetrieveAllMiddlemen();
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
            _OnDayStart.Invoke(middleman, _currentDay);
        }
        ChangeMiddlemanOrder();
        _currentDay++;
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
