using ClassLibrary.Containers.Abstractions;
using ClassLibrary.Entities;

namespace ClassLibrary.Containers;

public class BankAccountContainer : IBankAccountContainer
{
    private readonly Dictionary<Guid, BankAccount> _accounts = new Dictionary<Guid, BankAccount>();

    public void Add(BankAccount account)
    {
        _accounts[account.Id] = account;
    }

    public BankAccount GetById(Guid id)
    {
        _accounts.TryGetValue(id, out var account);
        return account;
    }

    public IEnumerable<BankAccount> GetAll()
    {
        return _accounts.Values;
    }

    public void Update(BankAccount account)
    {
        _accounts[account.Id] = account;
    }

    public void Remove(Guid id)
    {
        _accounts.Remove(id);
    }
}