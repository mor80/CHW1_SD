using ClassLibrary.Entities;

namespace ClassLibrary.Containers.Abstractions;

public interface ICategoryContainer
{
    void Add(Category category);
    Category GetById(Guid id);
    IEnumerable<Category> GetAll();
    void Update(Category category);
    void Remove(Guid id);
}