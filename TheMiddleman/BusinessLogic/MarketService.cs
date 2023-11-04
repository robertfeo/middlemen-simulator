using TheMiddleman.Entity;

public class MarketService
{
    private readonly ProductService _productService;
    private readonly MiddlemanService _middlemanService;
    public Action<Middleman, int> OnDayStart { get; set; } = delegate { };
    public Action<int> OnDayChange { get; set; } = delegate { };
    public int currentDay = 1;
    private List<Middleman> _middlemen;

    public MarketService()
    {
        _productService = new ProductService();
        _middlemanService = new MiddlemanService();
        _middlemen = _middlemanService.GetAllMiddlemen();
    }

    public MiddlemanService getMiddlemanService()
    {
        return _middlemanService;
    }

    public ProductService getProductService()
    {
        return _productService;
    }

    public void SimulateDay()
    {
        if (currentDay > 1)
        {
            _productService.CalculateProductAvailability();
        }
        foreach (var middleman in _middlemen)
        {
            OnDayStart.Invoke(middleman, currentDay);
        }
        RotateMiddlemen();
        currentDay++;
    }

    private void RotateMiddlemen()
    {
        if (_middlemen.Count > 1)
        {
            var firstMiddleman = _middlemen[0];
            _middlemen.RemoveAt(0);
            _middlemen.Add(firstMiddleman);
        }
    }
}
