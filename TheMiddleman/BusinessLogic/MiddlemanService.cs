using TheMiddleman.Entity;

public class MiddlemanService
{
    public void ProcessPurchase(Middleman middleman, Product product, int quantity)
    {
        int totalCost = quantity * product.BasePrice;
        if (CanPurchase(middleman, product, quantity, totalCost))
        {
            product.AvailableQuantity -= quantity;
            middleman.AccountBalance -= totalCost;
            middleman.AddToWarehouse(product, quantity);
        }
    }

    public void ProcessSale(Middleman middleman, Product product, int quantity)
    {
        middleman.SellFromWarehouse(product, quantity);
        middleman.AccountBalance += quantity * product.SellingPrice;
    }

    private bool CanPurchase(Middleman middleman, Product product, int quantity, int totalCost)
    {
        return middleman.AccountBalance >= totalCost && product.AvailableQuantity >= quantity && middleman.CanStore(quantity);
    }
}
