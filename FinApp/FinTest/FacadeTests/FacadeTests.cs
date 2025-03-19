using ClassLibrary.Containers.Abstractions;
using ClassLibrary.Entities;
using ClassLibrary.Factories.Abstractions;
using FinApp.Facades;
using FinApp.Observers.Abstractions;
using Moq;

namespace FinTest.FacadeTests;

public class FacadeTests
{
    [Fact]
    public void create_bank_account_with_valid_parameters_returns_new_account()
    {
        var bankAccountContainer = new Mock<IBankAccountContainer>();
        var categoryContainer = new Mock<ICategoryContainer>();
        var operationContainer = new Mock<IOperationContainer>();
        var factory = new Mock<IDomainFactory>();
        var operationObservers = new List<IOperationObserver>();

        var expectedAccount = new BankAccount(Guid.NewGuid(), "Checking Account", 1000m);
        factory.Setup(f => f.CreateBankAccount("Checking Account", 1000m)).Returns(expectedAccount);

        var financeFacade = new FinanceFacade(
            bankAccountContainer.Object,
            categoryContainer.Object,
            operationContainer.Object,
            factory.Object,
            operationObservers);

        var result = financeFacade.CreateBankAccount("Checking Account", 1000m);

        Assert.Equal(expectedAccount, result);
        bankAccountContainer.Verify(c => c.Add(expectedAccount), Times.Once);
    }


    [Fact]
    public void create_category_with_valid_parameters_returns_new_category()
    {
        var bankAccountContainer = new Mock<IBankAccountContainer>();
        var categoryContainer = new Mock<ICategoryContainer>();
        var operationContainer = new Mock<IOperationContainer>();
        var factory = new Mock<IDomainFactory>();
        var operationObservers = new List<IOperationObserver>();

        var expectedCategory = new Category(Guid.NewGuid(), CategoryType.Expense, "Groceries");
        factory.Setup(f => f.CreateCategory(CategoryType.Expense, "Groceries")).Returns(expectedCategory);

        var financeFacade = new FinanceFacade(
            bankAccountContainer.Object,
            categoryContainer.Object,
            operationContainer.Object,
            factory.Object,
            operationObservers);

        var result = financeFacade.CreateCategory(CategoryType.Expense, "Groceries");

        Assert.Equal(expectedCategory, result);
        categoryContainer.Verify(c => c.Add(expectedCategory), Times.Once);
    }


    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void create_bank_account_with_empty_name_throws_argument_exception(string invalidName)
    {
        var bankAccountContainer = new Mock<IBankAccountContainer>();
        var categoryContainer = new Mock<ICategoryContainer>();
        var operationContainer = new Mock<IOperationContainer>();
        var factory = new Mock<IDomainFactory>();
        var operationObservers = new List<IOperationObserver>();

        var financeFacade = new FinanceFacade(
            bankAccountContainer.Object,
            categoryContainer.Object,
            operationContainer.Object,
            factory.Object,
            operationObservers);

        var exception = Assert.Throws<ArgumentException>(() => financeFacade.CreateBankAccount(invalidName, 1000m));

        Assert.Equal("name", exception.ParamName);
        Assert.Contains("Название не может быть пустым", exception.Message);
        factory.Verify(f => f.CreateBankAccount(It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
        bankAccountContainer.Verify(c => c.Add(It.IsAny<BankAccount>()), Times.Never);
    }


    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void create_category_with_empty_name_throws_argument_exception(string invalidName)
    {
        var bankAccountContainer = new Mock<IBankAccountContainer>();
        var categoryContainer = new Mock<ICategoryContainer>();
        var operationContainer = new Mock<IOperationContainer>();
        var factory = new Mock<IDomainFactory>();
        var operationObservers = new List<IOperationObserver>();

        var financeFacade = new FinanceFacade(
            bankAccountContainer.Object,
            categoryContainer.Object,
            operationContainer.Object,
            factory.Object,
            operationObservers);

        var exception = Assert.Throws<ArgumentException>(() =>
            financeFacade.CreateCategory(CategoryType.Income, invalidName));

        Assert.Equal("name", exception.ParamName);
        Assert.Contains("Название не может быть пустым", exception.Message);
        factory.Verify(f => f.CreateCategory(It.IsAny<CategoryType>(), It.IsAny<string>()), Times.Never);
        categoryContainer.Verify(c => c.Add(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public void create_operation_with_valid_parameters_returns_new_operation_and_updates_balance()
    {
        // Arrange
        var bankAccountContainer = new Mock<IBankAccountContainer>();
        var categoryContainer = new Mock<ICategoryContainer>();
        var operationContainer = new Mock<IOperationContainer>();
        var factory = new Mock<IDomainFactory>();
        var operationObserverMock = new Mock<IOperationObserver>();
        var operationObservers = new List<IOperationObserver> { operationObserverMock.Object };

        var bankAccountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var expectedOperation = new Operation(Guid.NewGuid(), OperationType.Income, bankAccountId, 500m, DateTime.Now,
            "Salary", categoryId);
        factory.Setup(f =>
                f.CreateOperation(OperationType.Income, bankAccountId, 500m, It.IsAny<DateTime>(), "Salary",
                    categoryId))
            .Returns(expectedOperation);

        // Подготавливаем банк. счет с начальным балансом 1000
        var account = new BankAccount(bankAccountId, "Account1", 1000m);
        bankAccountContainer.Setup(b => b.GetById(bankAccountId)).Returns(account);

        var financeFacade = new FinanceFacade(
            bankAccountContainer.Object,
            categoryContainer.Object,
            operationContainer.Object,
            factory.Object,
            operationObservers);

        // Act
        var result = financeFacade.CreateOperation(OperationType.Income, bankAccountId, 500m, DateTime.Now, "Salary",
            categoryId);

        // Assert
        Assert.Equal(expectedOperation, result);
        operationContainer.Verify(o => o.Add(expectedOperation), Times.Once);
        // Для доходной операции баланс должен увеличиться: 1000 + 500 = 1500
        Assert.Equal(1500m, account.Balance);
        bankAccountContainer.Verify(b => b.Update(account), Times.Once);
        operationObserverMock.Verify(o => o.OnOperationCreated(expectedOperation), Times.Once);
    }

    // Редактирование счета: смена имени
    [Fact]
    public void update_bank_account_updates_name()
    {
        // Arrange
        var bankAccountContainer = new Mock<IBankAccountContainer>();
        var categoryContainer = new Mock<ICategoryContainer>();
        var operationContainer = new Mock<IOperationContainer>();
        var factory = new Mock<IDomainFactory>();
        var operationObservers = new List<IOperationObserver>();

        var accountId = Guid.NewGuid();
        var account = new BankAccount(accountId, "OldName", 1000m);
        bankAccountContainer.Setup(b => b.GetById(accountId)).Returns(account);

        var financeFacade = new FinanceFacade(
            bankAccountContainer.Object,
            categoryContainer.Object,
            operationContainer.Object,
            factory.Object,
            operationObservers);

        // Act
        var updatedAccount = financeFacade.UpdateBankAccount(accountId, "NewName");

        // Assert
        Assert.Equal("NewName", updatedAccount.Name);
        bankAccountContainer.Verify(b => b.Update(account), Times.Once);
    }

    // Редактирование категории: смена имени и типа
    [Fact]
    public void update_category_updates_name_and_type()
    {
        // Arrange
        var bankAccountContainer = new Mock<IBankAccountContainer>();
        var categoryContainer = new Mock<ICategoryContainer>();
        var operationContainer = new Mock<IOperationContainer>();
        var factory = new Mock<IDomainFactory>();
        var operationObservers = new List<IOperationObserver>();

        var categoryId = Guid.NewGuid();
        var category = new Category(categoryId, CategoryType.Expense, "OldCategory");
        categoryContainer.Setup(c => c.GetById(categoryId)).Returns(category);

        var financeFacade = new FinanceFacade(
            bankAccountContainer.Object,
            categoryContainer.Object,
            operationContainer.Object,
            factory.Object,
            operationObservers);

        // Act
        var updatedCategory = financeFacade.UpdateCategory(categoryId, "NewCategory", CategoryType.Income);

        // Assert
        Assert.Equal("NewCategory", updatedCategory.Name);
        Assert.Equal(CategoryType.Income, updatedCategory.Type);
        categoryContainer.Verify(c => c.Update(category), Times.Once);
    }

    // Редактирование операции: изменение суммы, типа, описания, даты и категории с корректировкой баланса
    [Fact]
    public void update_operation_updates_fields_and_updates_balance()
    {
        // Arrange
        var bankAccountContainer = new Mock<IBankAccountContainer>();
        var categoryContainer = new Mock<ICategoryContainer>();
        var operationContainer = new Mock<IOperationContainer>();
        var factory = new Mock<IDomainFactory>();
        var operationObservers = new List<IOperationObserver>();

        var accountId = Guid.NewGuid();
        // Создаем счет с балансом 1000
        var account = new BankAccount(accountId, "Account1", 1000m);
        // Симулируем, что операция доход 200 уже была создана и обновила баланс
        account.UpdateBalance(200m); // баланс становится 1200
        bankAccountContainer.Setup(b => b.GetById(accountId)).Returns(account);

        var operationId = Guid.NewGuid();
        // Исходная операция: доход 200
        var oldOperation = new Operation(operationId, OperationType.Income, accountId, 200m, DateTime.Now.AddDays(-1),
            "OldOp", Guid.NewGuid());
        operationContainer.Setup(o => o.GetById(operationId)).Returns(oldOperation);

        var financeFacade = new FinanceFacade(
            bankAccountContainer.Object,
            categoryContainer.Object,
            operationContainer.Object,
            factory.Object,
            operationObservers);

        // Act
        // Обновляем операцию: теперь Expense 300, новое описание, новая дата и новая категория
        var newCategoryId = Guid.NewGuid();
        var newDate = DateTime.Now;
        var updatedOperation =
            financeFacade.UpdateOperation(operationId, OperationType.Expense, 300m, newDate, "NewOp", newCategoryId);

        // Assert
        Assert.NotNull(updatedOperation);
        Assert.Equal(OperationType.Expense, updatedOperation.Type);
        Assert.Equal(300m, updatedOperation.Amount);
        Assert.Equal(newDate, updatedOperation.Date);
        Assert.Equal("NewOp", updatedOperation.Description);
        Assert.Equal(newCategoryId, updatedOperation.CategoryId);

        // Баланс:
        // До обновления: 1000 + 200 = 1200
        // Отмена исходной операции: 1200 - 200 = 1000
        // Применение новой операции (расход 300): 1000 - 300 = 700
        Assert.Equal(700m, account.Balance);
        bankAccountContainer.Verify(b => b.Update(account), Times.Once);
        operationContainer.Verify(o => o.Update(oldOperation), Times.Once);
    }

    // Удаление счета: возвращается true, если счет существует
    [Fact]
    public void delete_bank_account_returns_true_when_account_exists()
    {
        // Arrange
        var bankAccountContainer = new Mock<IBankAccountContainer>();
        var categoryContainer = new Mock<ICategoryContainer>();
        var operationContainer = new Mock<IOperationContainer>();
        var factory = new Mock<IDomainFactory>();
        var operationObservers = new List<IOperationObserver>();

        var accountId = Guid.NewGuid();
        var account = new BankAccount(accountId, "Account1", 1000m);
        bankAccountContainer.Setup(b => b.GetById(accountId)).Returns(account);

        var financeFacade = new FinanceFacade(
            bankAccountContainer.Object,
            categoryContainer.Object,
            operationContainer.Object,
            factory.Object,
            operationObservers);

        // Act
        var result = financeFacade.DeleteBankAccount(accountId);

        // Assert
        Assert.True(result);
        bankAccountContainer.Verify(b => b.Remove(accountId), Times.Once);
    }

    // Удаление категории: возвращается true, если категория существует
    [Fact]
    public void delete_category_returns_true_when_category_exists()
    {
        // Arrange
        var bankAccountContainer = new Mock<IBankAccountContainer>();
        var categoryContainer = new Mock<ICategoryContainer>();
        var operationContainer = new Mock<IOperationContainer>();
        var factory = new Mock<IDomainFactory>();
        var operationObservers = new List<IOperationObserver>();

        var categoryId = Guid.NewGuid();
        var category = new Category(categoryId, CategoryType.Income, "Category1");
        categoryContainer.Setup(c => c.GetById(categoryId)).Returns(category);

        var financeFacade = new FinanceFacade(
            bankAccountContainer.Object,
            categoryContainer.Object,
            operationContainer.Object,
            factory.Object,
            operationObservers);

        // Act
        var result = financeFacade.DeleteCategory(categoryId);

        // Assert
        Assert.True(result);
        categoryContainer.Verify(c => c.Remove(categoryId), Times.Once);
    }

    // Удаление операции: возвращается true и баланс обновляется корректно
    [Fact]
    public void delete_operation_returns_true_and_updates_balance()
    {
        // Arrange
        var bankAccountContainer = new Mock<IBankAccountContainer>();
        var categoryContainer = new Mock<ICategoryContainer>();
        var operationContainer = new Mock<IOperationContainer>();
        var factory = new Mock<IDomainFactory>();
        var operationObservers = new List<IOperationObserver>();

        var accountId = Guid.NewGuid();
        // Банк. счет с балансом 1200
        var account = new BankAccount(accountId, "Account1", 1200m);
        bankAccountContainer.Setup(b => b.GetById(accountId)).Returns(account);

        var operationId = Guid.NewGuid();
        // Операция: расход 200
        var operation = new Operation(operationId, OperationType.Expense, accountId, 200m, DateTime.Now, "ExpenseOp",
            Guid.NewGuid());
        operationContainer.Setup(o => o.GetById(operationId)).Returns(operation);

        var financeFacade = new FinanceFacade(
            bankAccountContainer.Object,
            categoryContainer.Object,
            operationContainer.Object,
            factory.Object,
            operationObservers);

        // Act
        var result = financeFacade.DeleteOperation(operationId);

        // Assert
        Assert.True(result);
        // При удалении расходной операции баланс увеличивается на сумму операции: 1200 + 200 = 1400.
        Assert.Equal(1400m, account.Balance);
        bankAccountContainer.Verify(b => b.Update(account), Times.Once);
        operationContainer.Verify(o => o.Remove(operationId), Times.Once);
    }
}