using ClassLibrary.Entities;

namespace ClassLibrary.Containers.Abstractions;

public interface IBankAccountContainer
{
    void Add(BankAccount account);
    BankAccount GetById(Guid id);
    IEnumerable<BankAccount> GetAll();
    void Update(BankAccount account);
    void Remove(Guid id);
}