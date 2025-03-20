using ClassLibrary.Containers.Abstractions;
using ClassLibrary.Entities;

namespace ClassLibrary.Containers;

public class CategoryContainer : ICategoryContainer
{
    private readonly Dictionary<Guid, Category> _categories = new Dictionary<Guid, Category>();

    public void Add(Category category)
    {
        _categories[category.Id] = category;
    }

    public Category GetById(Guid id)
    {
        _categories.TryGetValue(id, out var category);
        return category;
    }

    public IEnumerable<Category> GetAll()
    {
        return _categories.Values;
    }

    public void Update(Category category)
    {
        _categories[category.Id] = category;
    }

    public void Remove(Guid id)
    {
        _categories.Remove(id);
    }
}