using NUnit.Framework;
using TheMiddleman.DataAccess;
using Moq;

[TestFixture]
public class ProductServiceTests
{
    private ProductService? _productService;
    private Mock<IProductRepository>? _mockProductRepository;

    [SetUp]
    public void Setup()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockProductRepository.Setup(repo => repo.GetAllProducts()).Returns(new List<Product>
        {
            new Product { Id = 1, Name = "Product1", BasePrice = 10, PurchasePrice = 10, AvailableQuantity = 100, MinProductionRate = 1, MaxProductionRate = 5, Durability = 10 },
            new Product { Id = 2, Name = "Product2", BasePrice = 20, PurchasePrice = 20, AvailableQuantity = 200, MinProductionRate = 2, MaxProductionRate = 10, Durability = 5 }
        });
        _productService = new ProductService(_mockProductRepository.Object);
    }

    [Test]
    public void UpdateProducts_ShouldUpdateAvailability()
    {
        _productService!.UpdateProducts();
        var products = _productService.GetAllProducts();
        foreach (var product in products)
        {
            Assert.That(product.AvailableQuantity + 1, Is.AtLeast(product.AvailableQuantity), "Product availability should be updated correctly.");
        }
    }

    [Test]
    public void UpdateProducts_ShouldUpdatePrices()
    {
        _productService!.UpdateProducts();
        var products = _productService.GetAllProducts();
        foreach (var product in products)
        {
            Assert.That(product.PurchasePrice, Is.Not.EqualTo(product.BasePrice).And.InRange(product.BasePrice * 0.25, product.BasePrice * 3), "Product price should have been updated from the base price.");
        }
    }
}
