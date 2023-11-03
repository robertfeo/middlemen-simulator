namespace TheMiddleman.DataAccess
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _filePath;

        public ProductRepository(string filePath)
        {
            _filePath = filePath;
        }

        public string ReadProductName(string line)
        {
            return line.Substring(8);
        }

        public int ReadProductDurability(string line)
        {
            return int.Parse(line.Substring(14));
        }

        public int ReadProductBasePrice(string line)
        {
            return int.Parse(line.Substring(13));
        }

        public Product CreateProduct(int id, string name, int durability)
        {
            return new Product { Id = id, Name = name, Durability = durability };
        }

        public List<Product> GetAllProducts()
        {
            string[] lines = File.ReadAllLines(_filePath);
            List<Product> products = new List<Product>();
            Product? currentProduct = null;
            int idCounter = 1;
            foreach (var line in lines)
            {
                if (line.StartsWith("- name: "))
                {
                    string name = ReadProductName(line);
                    currentProduct = CreateProduct(idCounter++, name, 0);
                }
                else if (line.StartsWith("  durability: "))
                {
                    if (currentProduct != null)
                    {
                        int durability = ReadProductDurability(line);
                        currentProduct.Durability = durability;
                        products.Add(currentProduct);
                    }
                }
                else if (line.StartsWith("  baseprice: "))
                {
                    int basePrice = ReadProductBasePrice(line);
                    if (currentProduct != null)
                    {
                        currentProduct.BasePrice = basePrice;
                    }
                }
                else if (line.StartsWith("  minProductionRate: "))
                {
                    int minProductionRate = int.Parse(line.Substring(20));
                    if (currentProduct != null)
                    {
                        currentProduct.MinProductionRate = minProductionRate;
                    }
                }
                else if (line.StartsWith("  maxProductionRate: "))
                {
                    int maxProductionRate = int.Parse(line.Substring(20));
                    if (currentProduct != null)
                    {
                        currentProduct.MaxProductionRate = maxProductionRate;
                    }
                }
            }
            return products;
        }
    }
}
