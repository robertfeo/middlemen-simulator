using TheMiddleman.DataAccess;

public class ProductService
{
    private ProductRepository _productRepository;

    public ProductService()
    {
        _productRepository = new ProductRepository();
        _productRepository.CreateProducts();
    }

    public void UpdateProducts()
    {
        CalculateProductAvailability();
        UpdateProductPrices();
    }

    private void CalculateProductAvailability()
    {
        foreach (Product product in _productRepository.GetAllProducts())
        {
            int maxAvailability = product.MaxProductionRate * product.Durability;
            int productionToday = (int)RandomValueBetween(product.MinProductionRate, product.MaxProductionRate + 1);
            product.AvailableQuantity += productionToday;
            product.AvailableQuantity = Math.Max(0, product.AvailableQuantity);
            product.AvailableQuantity = Math.Min(maxAvailability, product.AvailableQuantity);
        }
    }

    private void UpdateProductPrices()
    {
        var products = _productRepository.GetAllProducts();
        foreach (var product in products)
        {
            double maxAvailability = product.MaxProductionRate * product.Durability;
            double availabilityPercentage = product.AvailableQuantity / maxAvailability;
            product.PurchasePrice = (int)CalculateNewProductPrice(product, availabilityPercentage);
        }
    }

    private double CalculateNewProductPrice(Product product, double availabilityPercentage)
    {
        double priceChangePercentage;
        double newPurchasePrice;
        if (availabilityPercentage < 0.25)
        {
            priceChangePercentage = RandomValueBetween(-0.10, 0.30);
            newPurchasePrice = product.BasePrice * (1 + priceChangePercentage);
        }
        else if (availabilityPercentage >= 0.25 && availabilityPercentage <= 0.80)
        {
            priceChangePercentage = RandomValueBetween(-0.05, 0.05);
            newPurchasePrice = product.BasePrice * (1 + priceChangePercentage);
        }
        else
        {
            priceChangePercentage = RandomValueBetween(-0.10, 0.06);
            newPurchasePrice = product.BasePrice * (1 + priceChangePercentage);
        }
        newPurchasePrice = Math.Max(newPurchasePrice, product.BasePrice * 0.25);
        newPurchasePrice = Math.Min(newPurchasePrice, product.BasePrice * 3);
        return newPurchasePrice;
    }

    private double RandomValueBetween(double minValue, double maxValue)
    {
        Random random = new Random();
        return minValue + (random.NextDouble() * (maxValue - minValue));
    }

    public List<Product> GetAllProducts()
    {
        return _productRepository.GetAllProducts();
    }

    public Product? FindProductById(int productId)
    {
        var _products = _productRepository.GetAllProducts();
        if(_products.Any(p => p.Id == productId))
        {
            return _products.First(p => p.Id == productId);
        }
        else
        {
            ConsoleUI.ShowErrorLog("Produkt nicht gefunden!");
            return null;
        }
    }
}
