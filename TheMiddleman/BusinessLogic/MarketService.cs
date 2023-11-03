using TheMiddleman.Entity;

public class MarketService
{
    private readonly ProductService _productService;
    private readonly MiddlemanService _middlemanService;

    public MarketService(ProductService productService, MiddlemanService middlemanService)
    {
        _productService = productService;
        _middlemanService = middlemanService;
    }

    public void SimulateDay(List<Middleman> middlemen, List<Product> products, int currentDay)
    {
        if (currentDay > 1)
        {
            _productService.CalculateProductAvailability(products);
        }
        foreach (var middleman in middlemen)
        {
            ShowMenuAndTakeAction(middleman, ref currentDay, products);
        }
        RotateMiddlemen(middlemen);
        currentDay++;
    }

    private static void RotateMiddlemen(List<Middleman> middlemen)
    {
        if (middlemen.Count > 1)
        {
            var firstMiddleman = middlemen[0];
            middlemen.RemoveAt(0);
            middlemen.Add(firstMiddleman);
        }
    }
}
