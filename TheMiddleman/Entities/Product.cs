public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Durability { get; set; }
    public double BasePrice { get; set; }
    public double PurchasePrice { get; set; }
    public double SellingPrice => (double)Math.Round(BasePrice * 0.8);
    public int MinProductionRate { get; set; }
    public int MaxProductionRate { get; set; }
    public int AvailableQuantity { get; set; } = 0;
}