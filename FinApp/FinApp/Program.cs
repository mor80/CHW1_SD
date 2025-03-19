using ConsoleApp.UI;
using System.Collections.Generic;
using ClassLibrary.Containers;
using ClassLibrary.Containers.Abstractions;
using ClassLibrary.Factories;
using ClassLibrary.Factories.Abstractions;
using FinApp.Facades;
using FinApp.Facades.Abstractions;
using FinApp.Observers;
using FinApp.Observers.Abstractions;
using Microsoft.Extensions.DependencyInjection;


namespace ConsoleApp;

public class Program
{
    static void Main(string[] args)
    {
        var serviceProvider = ConfigureServices();
        var programUi = new ProgramUI(serviceProvider);
        programUi.Run();
    }

    public static IServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();

        // Регистрация фабрики.
        serviceCollection.AddSingleton<IDomainFactory, DomainFactory>();

        // Регистрация контейнеров.
        serviceCollection.AddSingleton<IBankAccountContainer, BankAccountContainer>();
        serviceCollection.AddSingleton<ICategoryContainer, CategoryContainer>();
        serviceCollection.AddSingleton<IOperationContainer, OperationContainer>();

        // Регистрация наблюдателей (паттерн Observer).
        serviceCollection.AddSingleton<IOperationObserver, ConsoleOperationObserver>();

        // Регистрация фасада.
        serviceCollection.AddSingleton<IFinanceFacade, FinanceFacade>();

        return serviceCollection.BuildServiceProvider();
    }
}