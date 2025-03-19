using ClassLibrary.Entities;
using ConsoleApp.Observers.Abstractions;

namespace ConsoleApp.Observers;

public class ConsoleOperationObserver : IOperationObserver
{
    public void OnOperationCreated(Operation operation)
    {
        Console.WriteLine($"Оповещение: создана операция с ID {operation.Id} и суммой {operation.Amount}");
    }
}