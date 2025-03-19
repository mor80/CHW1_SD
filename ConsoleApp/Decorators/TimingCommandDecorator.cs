using System.Diagnostics;
using ConsoleApp.Commands.Abstractions;

namespace ConsoleApp.Decorators;

public class TimingCommandDecorator : ICommand
{
    private readonly ICommand _innerCommand;

    public TimingCommandDecorator(ICommand innerCommand)
    {
        _innerCommand = innerCommand;
    }

    public void Execute()
    {
        var stopwatch = Stopwatch.StartNew();
        _innerCommand.Execute();
        stopwatch.Stop();
        Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} мс");
    }
}