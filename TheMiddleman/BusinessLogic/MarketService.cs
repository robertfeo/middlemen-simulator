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
        // The logic for rotating middlemen and simulating actions can be moved here.
    }
}
