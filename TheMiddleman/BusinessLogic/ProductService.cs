using TheMiddleman.DataAccess;

public class ProductService
{
    private ProductRepository productRepository;

    public ProductService()
    {
        productRepository = new ProductRepository();
        productRepository.InitializeAllProducts();
    }

    public void UpdateProducts()
    {
        CalculateProductAvailability();
        UpdateProductPrices();
    }

    private void CalculateProductAvailability()
    {
        foreach (Product product in productRepository.GetAllProducts())
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
        var products = productRepository.GetAllProducts();
        foreach (var product in products)
        {
            double maxAvailability = product.MaxProductionRate * product.Durability;
            double availabilityPercentage = product.AvailableQuantity / maxAvailability;
            product.PurchasePrice = (int)CalculateNewProductPrice(product, availabilityPercentage);
            //Console.WriteLine($"Produkt: {product.Name} | Verfügbarkeit: {Math.Round(availabilityPercentage * 100, 2)}% ({product.AvailableQuantity} / {maxAvailability})");
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
        //Console.WriteLine($"Preisänderung in Prozent: {Math.Round(priceChangePercentage * 100, 2)}%");
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
        return productRepository.GetAllProducts();
    }
}
