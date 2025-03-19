using ClassLibrary.Entities;

namespace FinApp.Facades.Abstractions;

public interface IFinanceFacade
{
    // Методы создания
    BankAccount CreateBankAccount(string name, decimal initialBalance);
    Category CreateCategory(CategoryType type, string name);

    Operation CreateOperation(OperationType type, Guid bankAccountId, decimal amount, DateTime date, string description,
        Guid categoryId);

    // Методы редактирования
    BankAccount UpdateBankAccount(Guid id, string newName);
    Category UpdateCategory(Guid id, string newName, CategoryType newType);

    Operation UpdateOperation(Guid id, OperationType newType, decimal newAmount, DateTime newDate,
        string newDescription, Guid newCategoryId);

    // Методы удаления
    bool DeleteBankAccount(Guid id);
    bool DeleteCategory(Guid id);
    bool DeleteOperation(Guid id);

    IEnumerable<Operation> GetOperationsByAccount(Guid bankAccountId);
    decimal GetIncomeExpenseDifference(DateTime start, DateTime end);
}