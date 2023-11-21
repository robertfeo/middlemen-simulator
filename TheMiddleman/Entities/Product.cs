public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Durability { get; set; }
    public int BasePrice { get; set; }
    public int PurchasePrice { get; set; }
    public int SellingPrice => (int) Math.Round(BasePrice * 0.8);
    public int MinProductionRate { get; set; }
    public int MaxProductionRate { get; set; }
    public int AvailableQuantity { get; set; } = 0;
}