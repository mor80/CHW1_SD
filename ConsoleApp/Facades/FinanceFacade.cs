using ClassLibrary.Containers.Abstractions;
using ClassLibrary.Entities;
using ClassLibrary.Factories.Abstractions;
using ConsoleApp.Facades.Abstractions;
using ConsoleApp.Observers.Abstractions;

namespace ConsoleApp.Facades;

public class FinanceFacade : IFinanceFacade
{
    private readonly IBankAccountContainer _bankAccountContainer;
    private readonly ICategoryContainer _categoryContainer;
    private readonly IOperationContainer _operationContainer;
    private readonly IDomainFactory _factory;
    private readonly IEnumerable<IOperationObserver> _operationObservers;

    public FinanceFacade(IBankAccountContainer bankAccountContainer,
        ICategoryContainer categoryContainer,
        IOperationContainer operationContainer,
        IDomainFactory factory,
        IEnumerable<IOperationObserver> operationObservers)
    {
        _bankAccountContainer = bankAccountContainer;
        _categoryContainer = categoryContainer;
        _operationContainer = operationContainer;
        _factory = factory;
        _operationObservers = operationObservers;
    }

    // Создание объектов
    public BankAccount CreateBankAccount(string name, decimal initialBalance)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название не может быть пустым", nameof(name));

        var account = _factory.CreateBankAccount(name, initialBalance);
        _bankAccountContainer.Add(account);
        return account;
    }

    public Category CreateCategory(CategoryType type, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название не может быть пустым", nameof(name));

        var category = _factory.CreateCategory(type, name);
        _categoryContainer.Add(category);
        return category;
    }

    public Operation CreateOperation(OperationType type, Guid bankAccountId, decimal amount, DateTime date,
        string description, Guid categoryId)
    {
        var operation = _factory.CreateOperation(type, bankAccountId, amount, date, description, categoryId);
        _operationContainer.Add(operation);

        // Обновляем баланс счета
        var account = _bankAccountContainer.GetById(bankAccountId);
        if (account != null)
        {
            if (type == OperationType.Income)
                account.UpdateBalance(amount);
            else if (type == OperationType.Expense)
                account.UpdateBalance(-amount);
            _bankAccountContainer.Update(account);
        }

        // Уведомляем наблюдателей
        foreach (var observer in _operationObservers)
        {
            observer.OnOperationCreated(operation);
        }

        return operation;
    }

    // Редактирование объектов
    public BankAccount UpdateBankAccount(Guid id, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Название не может быть пустым", nameof(newName));

        var account = _bankAccountContainer.GetById(id);
        if (account != null)
        {
            account.Name = newName;
            _bankAccountContainer.Update(account);
        }

        return account;
    }

    public Category UpdateCategory(Guid id, string newName, CategoryType newType)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Название не может быть пустым", nameof(newName));

        var category = _categoryContainer.GetById(id);
        if (category != null)
        {
            category.Name = newName;
            category.Type = newType;
            _categoryContainer.Update(category);
        }

        return category;
    }

    public Operation UpdateOperation(Guid id, OperationType newType, decimal newAmount, DateTime newDate,
        string newDescription, Guid newCategoryId)
    {
        if (newAmount < 0)
            throw new ArgumentException("Сумма не может быть отрицательной", nameof(newAmount));

        var operation = _operationContainer.GetById(id);
        if (operation == null)
            return null;

        // Корректировка баланса: сначала отменяем влияние старой операции
        var account = _bankAccountContainer.GetById(operation.BankAccountId);
        if (account != null)
        {
            if (operation.Type == OperationType.Income)
                account.UpdateBalance(-operation.Amount);
            else if (operation.Type == OperationType.Expense)
                account.UpdateBalance(operation.Amount);
        }

        // Обновляем поля операции
        operation.Type = newType;
        operation.Amount = newAmount;
        operation.Date = newDate;
        operation.Description = newDescription;
        operation.CategoryId = newCategoryId;

        // Применяем новое влияние операции
        if (account != null)
        {
            if (newType == OperationType.Income)
                account.UpdateBalance(newAmount);
            else if (newType == OperationType.Expense)
                account.UpdateBalance(-newAmount);
            _bankAccountContainer.Update(account);
        }

        _operationContainer.Update(operation);
        return operation;
    }

    // Удаление объектов
    public bool DeleteBankAccount(Guid id)
    {
        var account = _bankAccountContainer.GetById(id);
        if (account != null)
        {
            _bankAccountContainer.Remove(id);
            return true;
        }

        return false;
    }

    public bool DeleteCategory(Guid id)
    {
        var category = _categoryContainer.GetById(id);
        if (category != null)
        {
            _categoryContainer.Remove(id);
            return true;
        }

        return false;
    }

    public bool DeleteOperation(Guid id)
    {
        var operation = _operationContainer.GetById(id);
        if (operation == null)
            return false;

        // Отменяем влияние операции на баланс счета
        var account = _bankAccountContainer.GetById(operation.BankAccountId);
        if (account != null)
        {
            if (operation.Type == OperationType.Income)
                account.UpdateBalance(-operation.Amount);
            else if (operation.Type == OperationType.Expense)
                account.UpdateBalance(operation.Amount);
            _bankAccountContainer.Update(account);
        }

        _operationContainer.Remove(id);
        return true;
    }

    public IEnumerable<Operation> GetOperationsByAccount(Guid bankAccountId)
    {
        return _operationContainer.GetByAccountId(bankAccountId);
    }

    public decimal GetIncomeExpenseDifference(DateTime start, DateTime end)
    {
        var operations = _operationContainer.GetAll()
            .Where(op => op.Date >= start && op.Date <= end);
        decimal income = operations.Where(op => op.Type == OperationType.Income).Sum(op => op.Amount);
        decimal expense = operations.Where(op => op.Type == OperationType.Expense).Sum(op => op.Amount);
        return income - expense;
    }
}