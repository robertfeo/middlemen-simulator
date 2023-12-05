namespace TheMiddleman.DataAccess
{
    public interface IProductRepository
    {
        string ReadProductName(string line);
        int ReadProductDurability(string line);
        int ReadProductBasePrice(string line);
        Product CreateProduct(int id, string name, int durability);
        List<Product> GetAllProducts();
        List<Product> CreateProducts();
    }
}