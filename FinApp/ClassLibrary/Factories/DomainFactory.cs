using ClassLibrary.Entities;
using ClassLibrary.Factories.Abstractions;

namespace ClassLibrary.Factories;

public class DomainFactory : IDomainFactory
{
    public BankAccount CreateBankAccount(string name, decimal initialBalance)
    {
        return new BankAccount(Guid.NewGuid(), name, initialBalance);
    }

    public Category CreateCategory(CategoryType type, string name)
    {
        return new Category(Guid.NewGuid(), type, name);
    }

    public Operation CreateOperation(OperationType type, Guid bankAccountId, decimal amount, DateTime date,
        string description, Guid categoryId)
    {
        return new Operation(Guid.NewGuid(), type, bankAccountId, amount, date, description, categoryId);
    }
}