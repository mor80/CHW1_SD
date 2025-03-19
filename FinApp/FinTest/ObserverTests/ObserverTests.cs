using ClassLibrary.Entities;
using FinApp.Observers.Abstractions;

namespace FinTest.ObserverTests;

public class TestOperationObserver : IOperationObserver
{
    public List<Operation> ReceivedOperations { get; } = new List<Operation>();

    public void OnOperationCreated(Operation operation)
    {
        ReceivedOperations.Add(operation);
    }
}

public class ObserverTests
{
    [Fact]
    public void OperationObserver_ReceivesOperationNotification()
    {
        // Arrange
        var observer = new TestOperationObserver();
        var operation = new Operation(
            Guid.NewGuid(),
            OperationType.Income,
            Guid.NewGuid(),
            100m,
            DateTime.Now,
            "Test Notification",
            Guid.NewGuid()
        );

        // Act
        observer.OnOperationCreated(operation);

        // Assert
        Assert.Single(observer.ReceivedOperations);
        Assert.Equal(operation, observer.ReceivedOperations.First());
    }
}