using ClassLibrary.Entities.Abstractions;

namespace ClassLibrary.Entities;

public class BankAccount : IIdentifiable
{
    public Guid Id { get; }
    public string Name { get; set; }
    public decimal Balance { get; private set; }

    public BankAccount(Guid id, string name, decimal balance)
    {
        Id = id;
        Name = name;
        Balance = balance;
    }

    public void UpdateBalance(decimal amount)
    {
        Balance += amount;
    }
}