using ClassLibrary.Entities;

namespace ConsoleApp.Observers.Abstractions;

public interface IOperationObserver
{
    void OnOperationCreated(Operation operation);
}