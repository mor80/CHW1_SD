using ClassLibrary.Entities;
using ClassLibrary.Factories;

namespace FinTest.FactoryTests;

public class FactoryTests
{
    [Fact]
    public void CreateBankAccount_ReturnsAccountWithCorrectParameters()
    {
        // Arrange
        var factory = new DomainFactory();
        string name = "Test Account";
        decimal initialBalance = 100m;

        // Act
        var account = factory.CreateBankAccount(name, initialBalance);

        // Assert
        Assert.NotNull(account);
        Assert.Equal(name, account.Name);
        Assert.Equal(initialBalance, account.Balance);
        Assert.NotEqual(Guid.Empty, account.Id);
    }

    [Fact]
    public void CreateCategory_ReturnsCategoryWithCorrectParameters()
    {
        // Arrange
        var factory = new DomainFactory();
        string name = "Food";
        var type = CategoryType.Expense;

        // Act
        var category = factory.CreateCategory(type, name);

        // Assert
        Assert.NotNull(category);
        Assert.Equal(name, category.Name);
        Assert.Equal(type, category.Type);
        Assert.NotEqual(Guid.Empty, category.Id);
    }

    [Fact]
    public void CreateOperation_ReturnsOperationWithCorrectParameters()
    {
        // Arrange
        var factory = new DomainFactory();
        var bankAccountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var opType = OperationType.Income;
        decimal amount = 50m;
        DateTime date = DateTime.Now;
        string description = "Test Operation";

        // Act
        var operation = factory.CreateOperation(opType, bankAccountId, amount, date, description, categoryId);

        // Assert
        Assert.NotNull(operation);
        Assert.Equal(opType, operation.Type);
        Assert.Equal(bankAccountId, operation.BankAccountId);
        Assert.Equal(amount, operation.Amount);
        Assert.Equal(date, operation.Date);
        Assert.Equal(description, operation.Description);
        Assert.Equal(categoryId, operation.CategoryId);
        Assert.NotEqual(Guid.Empty, operation.Id);
    }

    [Fact]
    public void CreateOperation_WithNegativeAmount_ThrowsArgumentException()
    {
        // Arrange
        var factory = new DomainFactory();
        var bankAccountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var opType = OperationType.Expense;
        decimal amount = -10m;
        DateTime date = DateTime.Now;
        string description = "Invalid Operation";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            factory.CreateOperation(opType, bankAccountId, amount, date, description, categoryId)
        );
        Assert.Contains("Сумма не может быть отрицательной", exception.Message);
    }
}