using ClassLibrary.Entities.Abstractions;

namespace ClassLibrary.Entities;

public enum OperationType
{
    Income,
    Expense
}

public class Operation
{
    public Guid Id { get; }
    public OperationType Type { get; set; }
    public Guid BankAccountId { get; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public Guid CategoryId { get; set; }

    public Operation(Guid id, OperationType type, Guid bankAccountId, decimal amount, DateTime date, string description,
        Guid categoryId)
    {
        if (amount < 0)
            throw new ArgumentException("Сумма не может быть отрицательной", nameof(amount));
        Id = id;
        Type = type;
        BankAccountId = bankAccountId;
        Amount = amount;
        Date = date;
        Description = description;
        CategoryId = categoryId;
    }
}