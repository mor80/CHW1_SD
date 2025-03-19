using ClassLibrary.Entities;

namespace ClassLibrary.Factories.Abstractions;

public interface IDomainFactory
{
    BankAccount CreateBankAccount(string name, decimal initialBalance);
    Category CreateCategory(CategoryType type, string name);

    Operation CreateOperation(OperationType type, Guid bankAccountId, decimal amount, DateTime date, string description,
        Guid categoryId);
}