using ClassLibrary.Containers.Abstractions;
using ClassLibrary.Entities;

namespace ClassLibrary.Containers;

public class OperationContainer : IOperationContainer
{
    private readonly Dictionary<Guid, Operation> _operations = new Dictionary<Guid, Operation>();

    public void Add(Operation operation)
    {
        _operations[operation.Id] = operation;
    }

    public Operation GetById(Guid id)
    {
        _operations.TryGetValue(id, out var operation);
        return operation;
    }

    public IEnumerable<Operation> GetAll()
    {
        return _operations.Values;
    }

    public void Update(Operation operation)
    {
        _operations[operation.Id] = operation;
    }

    public void Remove(Guid id)
    {
        _operations.Remove(id);
    }

    public IEnumerable<Operation> GetByAccountId(Guid bankAccountId)
    {
        return _operations.Values.Where(o => o.BankAccountId == bankAccountId);
    }
}