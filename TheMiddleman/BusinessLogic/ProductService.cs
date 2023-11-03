public class ProductService
{
    private readonly Random random = new Random();

    public void CalculateProductAvailability(List<Product> products)
    {
        foreach (Product product in products)
        {
            int maxAvailability = product.MaxProductionRate * product.Durability;
            double weight = 0.3;
            int productionToday = (int)((weight * product.MaxProductionRate) + ((1 - weight) * random.Next(product.MinProductionRate, product.MaxProductionRate + 1)));

            product.AvailableQuantity += productionToday;
            product.AvailableQuantity = Math.Max(0, product.AvailableQuantity);
            product.AvailableQuantity = Math.Min(maxAvailability, product.AvailableQuantity);
        }
    }
}
