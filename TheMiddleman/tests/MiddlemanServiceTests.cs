using NUnit.Framework;
using NUnit.Framework.Legacy;
using TheMiddleman.Entity;

[TestFixture]
public class MiddlemanServiceTests
{
    private MiddlemanService? _middlemanService;
    private Product? _product;
    private Middleman? _middleman;

    [SetUp]
    public void Setup()
    {
        _middlemanService = new MiddlemanService();
        _product = new Product { Id = 1, Name = "Product", PurchasePrice = 100, AvailableQuantity = 100 };
        _middleman = new Middleman("TestMiddleman", "TestCompany", 10000);
        ClassicAssert.NotNull(_middlemanService);
        ClassicAssert.NotNull(_product);
        ClassicAssert.NotNull(_middleman);
    }

    [Test]
    public void IncreaseWarehouseCapacity_InsufficientFunds_ThrowsException()
    {
        _middleman!.AccountBalance = 100;
        int increaseAmount = 50;
        Assert.Throws<InsufficientFundsException>(() => _middlemanService!.IncreaseWarehouseCapacity(_middleman!, increaseAmount));
    }

    [Test]
    public void IncreaseWarehouseCapacity_SufficientFunds_SuccessfulIncrease()
    {
        _middleman!.AccountBalance = 10000;
        int increaseAmount = 50;
        _middlemanService!.IncreaseWarehouseCapacity(_middleman!, increaseAmount);
        ClassicAssert.AreEqual(100 + increaseAmount, _middleman.MaxStorageCapacity);
    }

    [Test]
    public void DeductStorageCosts_InsufficientFunds_ThrowsException()
    {
        _middleman!.AccountBalance = 0;
        Assert.Throws<InsufficientFundsException>(() =>
        {
            _middlemanService!.DeductStorageCosts(_middleman!);
        });
    }

    [Test]
    public void PurchaseProduct_SuccessfulPurchase_DecreasesAccountBalance()
    {
        _middlemanService!.PurchaseProduct(_middleman!, _product!, 5);
        ClassicAssert.AreEqual(9500, _middleman!.AccountBalance);
        ClassicAssert.AreEqual(95, _product!.AvailableQuantity);
        Assert.That(_middleman.Warehouse.ContainsKey(_product));
        ClassicAssert.AreEqual(5, _middleman.Warehouse[_product]);
        ClassicAssert.AreEqual(500, _middleman.DailyExpenses);
    }

    [Test]
    public void PurchaseProduct_SuccessfulPurchase_UpdatesAccountAndWarehouse()
    {
        int purchaseQuantity = 5;
        double initialAccountBalance = _middleman!.AccountBalance;
        double totalCost = purchaseQuantity * _product!.PurchasePrice;
        _middlemanService!.PurchaseProduct(_middleman!, _product!, purchaseQuantity);
        ClassicAssert.AreEqual(initialAccountBalance - totalCost, _middleman!.AccountBalance);
        ClassicAssert.IsTrue(_middleman!.Warehouse.ContainsKey(_product!));
        ClassicAssert.AreEqual(purchaseQuantity, _middleman!.Warehouse[_product!]);
    }

    [Test]
    public void PurchaseProduct_SuccessfulPurchase_UpdatesMarketAvailability()
    {
        int purchaseQuantity = 2;
        int initialProductQuantity = _product!.AvailableQuantity;
        _middlemanService!.PurchaseProduct(_middleman!, _product!, purchaseQuantity);
        ClassicAssert.AreEqual(initialProductQuantity - purchaseQuantity, _product!.AvailableQuantity);
    }

    [Test]
    public void PurchaseProduct_SuccessfulPurchase_UpdatesDailyReport()
    {
        int purchaseQuantity = 3;
        double totalCost = purchaseQuantity * _product!.PurchasePrice;
        _middlemanService!.PurchaseProduct(_middleman!, _product!, purchaseQuantity);
        ClassicAssert.AreEqual(totalCost, _middleman!.DailyExpenses);
    }

    [Test]
    public void CalculateDiscount_LargeQuantity_ReturnsExpectedDiscount()
    {
        _middlemanService!.PurchaseProduct(_middleman!, _product!, 80);
        double expectedDiscount = 0.10;
        double discount = _middlemanService.CalculateDiscount(_product!, _middleman!);
        ClassicAssert.AreEqual(expectedDiscount, discount);
    }

    [Test]
    public void PurchaseProduct_InsufficientFunds_ThrowsException()
    {
        ClassicAssert.Throws<InsufficientFundsException>(() =>
        {
            _middlemanService!.PurchaseProduct(_middleman!, _product!, 200);
        });
    }

    [Test]
    public void PurchaseProduct_InsufficientFunds_NoChangeInAccountAndWarehouse()
    {
        _middleman!.AccountBalance = 0;
        int initialProductQuantity = _product!.AvailableQuantity;
        Assert.Throws<InsufficientFundsException>(() =>
        {
            _middlemanService!.PurchaseProduct(_middleman!, _product!, 5);
        });
        ClassicAssert.AreEqual(0, _middleman!.AccountBalance);
        ClassicAssert.AreEqual(initialProductQuantity, _product!.AvailableQuantity);
    }

    [Test]
    public void PurchaseProduct_InsufficientProductQuantity_ThrowsException()
    {
        _product!.AvailableQuantity = 10;
        Assert.Throws<ProductNotAvailableException>(() =>
        {
            _middlemanService!.PurchaseProduct(_middleman!, _product!, 20);
        });
    }

    [Test]
    public void PurchaseProduct_InsufficientProductQuantity_NoPurchaseMade()
    {
        _product!.AvailableQuantity = 1;
        double initialAccountBalance = _middleman!.AccountBalance;
        int purchaseQuantity = 5;
        Assert.Throws<ProductNotAvailableException>(() =>
        {
            _middlemanService!.PurchaseProduct(_middleman!, _product!, purchaseQuantity);
        });
        ClassicAssert.AreEqual(initialAccountBalance, _middleman!.AccountBalance, "Account balance should remain unchanged.");
        ClassicAssert.AreEqual(1, _product!.AvailableQuantity, "Product quantity should remain unchanged.");
    }

    [Test]
    public void SellProduct_SuccessfulSale_IncreasesAccountBalanceAndUpdatesWarehouse()
    {
        _middlemanService!.PurchaseProduct(_middleman!, _product!, 10);
        double initialAccountBalance = _middleman!.AccountBalance;
        int sellQuantity = 5;
        _middlemanService.SellProduct(_middleman!, _product!, sellQuantity);
        ClassicAssert.AreEqual(initialAccountBalance + sellQuantity * _product!.SellingPrice, _middleman.AccountBalance);
        ClassicAssert.AreEqual(10 - sellQuantity, _middleman.Warehouse[_product]);
    }

    [Test]
    public void SellProduct_InsufficientProductQuantity_ThrowsException()
    {
        _middlemanService!.PurchaseProduct(_middleman!, _product!, 3);
        Assert.Throws<ProductNotAvailableException>(() =>
        {
            _middlemanService.SellProduct(_middleman!, _product!, 5);
        });
    }

    [Test]
    public void SellProduct_SuccessfulSale_UpdatesDailyReport()
    {
        int purchaseQuantity = 10;
        _middlemanService!.PurchaseProduct(_middleman!, _product!, purchaseQuantity);
        int sellingQuantity = 5;
        double expectedDailyEarnings = sellingQuantity * _product!.SellingPrice;
        _middlemanService.SellProduct(_middleman!, _product!, sellingQuantity);
        ClassicAssert.AreEqual(expectedDailyEarnings, _middleman!.DailyEarnings);
    }

    [Test]
    public void TestTakeOutLoan()
    {
        var middleman = _middleman;
        _middlemanService!.RegisterNewLoan(3, middleman!, 5000, 0.03);
        ClassicAssert.IsNotNull(middleman!.CurrentLoan);
    }

    [Test]
    public void TestLoanRepayment()
    {
        var loanAmount = 5000;
        var interestRate = 0.03;
        var currentDay = 1;
        var dueDay = currentDay + 7;
        var expectedRepaymentAmount = loanAmount * (1 + interestRate);
        _middlemanService!.RegisterNewLoan(currentDay, _middleman!, loanAmount, interestRate);
        for (int day = currentDay; day <= dueDay; day++)
        {
            if (_middlemanService.VerifyLoanDue(_middleman!, day))
            {
                _middlemanService.RepayLoan(day, _middleman!);
            }
        }
        ClassicAssert.IsNull(_middleman!.CurrentLoan, "Loan should be repaid and set to null.");
        ClassicAssert.AreEqual(15000 - expectedRepaymentAmount, _middleman.AccountBalance, "Account balance should decrease by the repayment amount.");
    }

    [Test]
    public void RegisterNewLoan_AlreadyExistingLoan_ThrowsException()
    {
        _middlemanService!.RegisterNewLoan(1, _middleman!, 5000, 0.05);
        Assert.Throws<LoanAlreadyExistsException>(() => _middlemanService.RegisterNewLoan(2, _middleman!, 3000, 0.04));
    }

    [Test]
    public void HandleLoanRepayment_InsufficientFunds_ThrowsException()
    {
        _middlemanService!.RegisterNewLoan(1, _middleman!, 5000, 0.05);
        _middleman!.AccountBalance = 100;
        Assert.Throws<InsufficientFundsException>(() =>
        {
            _middlemanService.RepayLoan(8, _middleman!);
        });
    }

    [Test]
    public void ResetDailyReport_ResetsValuesToZero()
    {
        _middlemanService!.PurchaseProduct(_middleman!, _product!, 2);
        _middlemanService.SellProduct(_middleman!, _product!, 1);
        _middlemanService.ResetDailyReport(_middleman!);
        ClassicAssert.AreEqual(0, _middleman!.DailyExpenses);
        ClassicAssert.AreEqual(0, _middleman.DailyEarnings);
        ClassicAssert.AreEqual(0, _middleman.DailyStorageCosts);
    }

    [Test]
    public void GetOwnedProducts_ReturnsCorrectProducts()
    {
        _middlemanService!.PurchaseProduct(_middleman!, _product!, 5);
        var ownedProducts = _middlemanService.GetOwnedProducts(_middleman!);
        ClassicAssert.IsTrue(ownedProducts.Contains(_product!));
    }
}
