using ClassLibrary.Entities;
using FinApp.Commands;
using FinApp.Facades.Abstractions;
using Moq;

namespace FinTest.CommandTests;

public class CommandTests
{
    [Fact]
    public void Execute_CallsFacadeCreateOperationWithCorrectParameters()
    {
        // Arrange
        var facadeMock = new Mock<IFinanceFacade>();
        var type = OperationType.Income;
        var bankAccountId = Guid.NewGuid();
        var amount = 100m;
        var date = DateTime.Now;
        var description = "Test Operation";
        var categoryId = Guid.NewGuid();

        var command = new CreateOperationCommand(
            facadeMock.Object,
            type,
            bankAccountId,
            amount,
            date,
            description,
            categoryId);

        // Act
        command.Execute();

        // Assert
        facadeMock.Verify(f => f.CreateOperation(
            type,
            bankAccountId,
            amount,
            date,
            description,
            categoryId), Times.Once);
    }
}
