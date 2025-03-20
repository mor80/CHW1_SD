using ClassLibrary.Entities;

namespace FinApp.Observers.Abstractions;

public interface IOperationObserver
{
    void OnOperationCreated(Operation operation);
}