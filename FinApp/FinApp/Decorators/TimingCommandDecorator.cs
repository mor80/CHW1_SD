using System.Diagnostics;
using FinApp.Commands.Abstractions;

namespace FinApp.Decorators;

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