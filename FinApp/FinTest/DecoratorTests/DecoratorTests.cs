using FinApp.Commands.Abstractions;
using FinApp.Decorators;

namespace FinTest.DecoratorTests;

public class DummyCommand : ICommand
{
    public bool Executed { get; private set; }

    public void Execute()
    {
        Executed = true;
    }
}

public class DecoratorTests
{
    [Fact]
    public void Execute_ExecutesInnerCommandAndPrintsElapsedTime()
    {
        // Arrange
        var dummy = new DummyCommand();
        var decorator = new TimingCommandDecorator(dummy);

        // Перехватываем вывод в консоль
        var originalOut = Console.Out;
        using (var sw = new StringWriter())
        {
            Console.SetOut(sw);

            // Act
            decorator.Execute();

            // Assert
            Assert.True(dummy.Executed, "Внутренняя команда должна быть выполнена.");
            var output = sw.ToString();
            Assert.Contains("Время выполнения:", output);
        }

        // Восстанавливаем стандартный вывод
        Console.SetOut(originalOut);
    }
}