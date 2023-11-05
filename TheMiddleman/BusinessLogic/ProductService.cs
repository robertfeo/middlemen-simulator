using TheMiddleman.DataAccess;

public class ProductService
{
    private Random random = new Random();
    private ProductRepository productRepository;

    public ProductService()
    {
        productRepository = new ProductRepository();
        productRepository.InitializeAllProducts();
    }

    public void CalculateProductAvailability()
    {
        foreach (Product product in productRepository.GetAllProducts())
        {
            int maxAvailability = product.MaxProductionRate * product.Durability;
            double weight = 0.8;
            int productionToday = (int)((weight * product.MaxProductionRate) + ((1 - weight) * random.Next(product.MinProductionRate, product.MaxProductionRate + 1)));
            product.AvailableQuantity += productionToday;
            product.AvailableQuantity = Math.Max(0, product.AvailableQuantity);
            product.AvailableQuantity = Math.Min(maxAvailability, product.AvailableQuantity);
        }
    }

    public List<Product> GetAllProducts()
    {
        return productRepository.GetAllProducts();
    }
}
