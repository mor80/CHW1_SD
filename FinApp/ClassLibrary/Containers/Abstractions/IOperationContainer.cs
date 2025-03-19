using ClassLibrary.Entities;

namespace ClassLibrary.Containers.Abstractions;

public interface IOperationContainer
{
    void Add(Operation operation);
    Operation GetById(Guid id);
    IEnumerable<Operation> GetAll();
    void Update(Operation operation);
    void Remove(Guid id);
    IEnumerable<Operation> GetByAccountId(Guid bankAccountId);
}