using ClassLibrary.Entities.Abstractions;

namespace ClassLibrary.Entities;

public enum CategoryType
{
    Income,
    Expense
}

public class Category
{
    public Guid Id { get; }
    public CategoryType Type { get; set; }
    public string Name { get; set; }

    public Category(Guid id, CategoryType type, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название не может быть пустым", nameof(name));
        Id = id;
        Type = type;
        Name = name;
    }
}