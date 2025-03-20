using ClassLibrary.Containers.Abstractions;
using ClassLibrary.Entities;
using FinApp.Commands;
using FinApp.Decorators;
using FinApp.Facades.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp.UI;

public class ProgramUI
{
    private readonly IFinanceFacade _facade;
    private readonly IServiceProvider _serviceProvider;

    public ProgramUI(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _facade = _serviceProvider.GetRequiredService<IFinanceFacade>();
    }

    public void Run()
    {
        ShowMainMenu();
    }

    /// <summary>
    /// Главное меню с 3 разделами.
    /// </summary>
    private void ShowMainMenu()
    {
        while (true)
        {
            var mainMenu = new ConsoleMenu(new List<MenuCell>
            {
                new MenuCell("Управление счетами", ShowAccountMenu),
                new MenuCell("Управление категориями", ShowCategoryMenu),
                new MenuCell("Управление операциями", ShowOperationMenu),
                new MenuCell("Выход", Exit)
            });
            mainMenu.Show();
        }
    }

    private void ShowAccountMenu()
    {
        while (true)
        {
            var accountMenu = new ConsoleMenu(new List<MenuCell>
            {
                new MenuCell("Создать новый счет", CreateBankAccount),
                new MenuCell("Редактировать счет", EditBankAccount),
                new MenuCell("Удалить счет", DeleteBankAccount),
                new MenuCell("Показать баланс счета", ShowBalance),
                new MenuCell("Назад", ShowMainMenu)
            });
            accountMenu.Show();
        }
    }

    private void CreateBankAccount()
    {
        Console.Clear();
        Console.Write("Введите название счета: ");
        string accountName = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(accountName))
        {
            Console.Write("Название не может быть пустым. Введите название счета: ");
            accountName = Console.ReadLine();
        }

        Console.Write("Введите начальный баланс: ");
        decimal initialBalance;
        while (!decimal.TryParse(Console.ReadLine(), out initialBalance))
        {
            Console.Write("Некорректное значение. Введите начальный баланс: ");
        }

        var account = _facade.CreateBankAccount(accountName, initialBalance);
        Console.WriteLine($"Счет создан. ID: {account.Id}");
        Pause();
    }

    private void EditBankAccount()
    {
        Console.Clear();
        var account = SelectBankAccount();
        if (account == null)
            return;
        Console.Write("Введите новое название счета: ");
        string newName = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(newName))
        {
            Console.Write("Название не может быть пустым. Введите новое название счета: ");
            newName = Console.ReadLine();
        }

        _facade.UpdateBankAccount(account.Id, newName);
        Console.WriteLine("Счет обновлен.");
        Pause();
    }

    private void DeleteBankAccount()
    {
        Console.Clear();
        var account = SelectBankAccount();
        if (account == null)
            return;
        Console.WriteLine("Вы действительно хотите удалить счет? (Y/N)");
        var key = Console.ReadKey();
        if (key.KeyChar == 'Y' || key.KeyChar == 'y')
        {
            _facade.DeleteBankAccount(account.Id);
            Console.WriteLine("\nСчет удален.");
        }
        else
        {
            Console.WriteLine("\nОперация отменена.");
        }

        Pause();
    }

    private void ShowBalance()
    {
        Console.Clear();
        var account = SelectBankAccount();
        if (account == null)
            return;
        Console.WriteLine($"Баланс счета '{account.Name}': {account.Balance}");
        Pause();
    }

    private BankAccount SelectBankAccount()
    {
        var bankAccountCont = _serviceProvider.GetRequiredService<IBankAccountContainer>();
        var accounts = bankAccountCont.GetAll().ToList();
        if (accounts.Count == 0)
        {
            Console.WriteLine("Нет доступных счетов.");
            Pause();
            return null;
        }

        Console.WriteLine("Выберите счет:");
        for (int i = 0; i < accounts.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {accounts[i].Name} (ID: {accounts[i].Id})");
        }

        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > accounts.Count)
        {
            Console.WriteLine("Некорректный выбор. Повторите:");
        }

        return accounts[choice - 1];
    }


    private void ShowCategoryMenu()
    {
        while (true)
        {
            var categoryMenu = new ConsoleMenu(new List<MenuCell>
            {
                new MenuCell("Создать категорию", CreateCategory),
                new MenuCell("Редактировать категорию", EditCategory),
                new MenuCell("Удалить категорию", DeleteCategory),
                new MenuCell("Назад", ShowMainMenu)
            });
            categoryMenu.Show();
        }
    }

    private void CreateCategory()
    {
        Console.Clear();
        Console.Write("Введите название категории: ");
        string categoryName = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(categoryName))
        {
            Console.Write("Название не может быть пустым. Введите название категории: ");
            categoryName = Console.ReadLine();
        }

        Console.Write("Выберите тип (Доход - введите 1, Расход введите 2): ");
        CategoryType type;
        var key = Console.ReadKey();
        Console.WriteLine();
        type = (key.KeyChar == '1') ? CategoryType.Income : CategoryType.Expense;
        var category = _facade.CreateCategory(type, categoryName);
        Console.WriteLine($"Категория создана. ID: {category.Id}");
        Pause();
    }

    private void EditCategory()
    {
        Console.Clear();
        var category = SelectCategory();
        if (category == null)
            return;
        Console.Write("Введите новое название категории: ");
        string newName = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(newName))
        {
            Console.Write("Название не может быть пустым. Введите новое название категории: ");
            newName = Console.ReadLine();
        }

        Console.Write("Выберите новый тип (Доход - введите 1, Расход введите 2): ");
        CategoryType newType;
        var key = Console.ReadKey();
        Console.WriteLine();
        newType = (key.KeyChar == '1') ? CategoryType.Income : CategoryType.Expense;
        _facade.UpdateCategory(category.Id, newName, newType);
        Console.WriteLine("Категория обновлена.");
        Pause();
    }

    private void DeleteCategory()
    {
        Console.Clear();
        var category = SelectCategory();
        if (category == null)
            return;
        Console.WriteLine("Вы действительно хотите удалить категорию? (Y/N)");
        var key = Console.ReadKey();
        if (key.KeyChar == 'Y' || key.KeyChar == 'y')
        {
            _facade.DeleteCategory(category.Id);
            Console.WriteLine("\nКатегория удалена.");
        }
        else
        {
            Console.WriteLine("\nОперация отменена.");
        }

        Pause();
    }

    private Category SelectCategory()
    {
        var categoryCont = _serviceProvider.GetRequiredService<ICategoryContainer>();
        var categories = categoryCont.GetAll().ToList();
        if (categories.Count == 0)
        {
            Console.WriteLine("Нет доступных категорий.");
            Pause();
            return null;
        }

        Console.WriteLine("Выберите категорию:");
        for (int i = 0; i < categories.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {categories[i].Name} (ID: {categories[i].Id})");
        }

        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > categories.Count)
        {
            Console.WriteLine("Некорректный выбор. Повторите:");
        }

        return categories[choice - 1];
    }

    private void ShowOperationMenu()
    {
        while (true)
        {
            var operationMenu = new ConsoleMenu(new List<MenuCell>
            {
                new MenuCell("Создать операцию", CreateOperation),
                new MenuCell("Редактировать операцию", EditOperation),
                new MenuCell("Удалить операцию", DeleteOperation),
                new MenuCell("Назад", ShowMainMenu)
            });
            operationMenu.Show();
        }
    }

    private void CreateOperation()
    {
        Console.Clear();
        Console.WriteLine("Создание операции");
        var account = SelectBankAccount();
        if (account == null)
            return;
        Console.Write("Введите сумму операции: ");
        decimal amount;
        while (!decimal.TryParse(Console.ReadLine(), out amount) || amount < 0)
        {
            Console.Write("Некорректное значение. Введите сумму операции: ");
        }

        Console.Write("Введите описание операции: ");
        string description = Console.ReadLine();
        var category = SelectCategory();
        if (category == null)
            return;
        Console.Write("Выберите тип операции (Доход - введите 1, Расход введите 2): ");
        OperationType opType;
        var opKey = Console.ReadKey();
        Console.WriteLine();
        opType = (opKey.KeyChar == '1') ? OperationType.Income : OperationType.Expense;
        var createOpCommand = new CreateOperationCommand(_facade,
            opType,
            account.Id,
            amount,
            DateTime.Now,
            description,
            category.Id);
        var timedCommand = new TimingCommandDecorator(createOpCommand);
        timedCommand.Execute();
        Console.WriteLine("Операция создана.");
        Pause();
    }

    private void EditOperation()
    {
        Console.Clear();
        var operation = SelectOperation();
        if (operation == null)
            return;
        Console.Write("Введите новую сумму операции: ");
        decimal newAmount;
        while (!decimal.TryParse(Console.ReadLine(), out newAmount) || newAmount < 0)
        {
            Console.Write("Некорректное значение. Введите новую сумму: ");
        }

        Console.Write("Введите новое описание операции: ");
        string newDescription = Console.ReadLine();
        Console.Write("Введите новую дату операции (yyyy-MM-dd): ");
        DateTime newDate;
        while (!DateTime.TryParse(Console.ReadLine(), out newDate))
        {
            Console.Write("Некорректная дата. Введите дату операции (yyyy-MM-dd): ");
        }

        Console.Write("Выберите новый тип операции (Доход - введите 1, Расход - введите 2): ");
        OperationType newType;
        var opKey = Console.ReadKey();
        Console.WriteLine();
        newType = (opKey.KeyChar == '1') ? OperationType.Income : OperationType.Expense;
        var category = SelectCategory();
        if (category == null)
            return;
        _facade.UpdateOperation(operation.Id, newType, newAmount, newDate, newDescription, category.Id);
        Console.WriteLine("Операция обновлена.");
        Pause();
    }

    private void DeleteOperation()
    {
        Console.Clear();
        var operation = SelectOperation();
        if (operation == null)
            return;
        Console.WriteLine("Вы действительно хотите удалить операцию? (Y/N)");
        var key = Console.ReadKey();
        if (key.KeyChar == 'Y' || key.KeyChar == 'y')
        {
            _facade.DeleteOperation(operation.Id);
            Console.WriteLine("\nОперация удалена.");
        }
        else
        {
            Console.WriteLine("\nОперация отменена.");
        }

        Pause();
    }

    private Operation SelectOperation()
    {
        var opCont = _serviceProvider.GetRequiredService<IOperationContainer>();
        var operations = opCont.GetAll().ToList();
        if (operations.Count == 0)
        {
            Console.WriteLine("Нет доступных операций.");
            Pause();
            return null;
        }

        Console.WriteLine("Выберите операцию:");
        for (int i = 0; i < operations.Count; i++)
        {
            Console.WriteLine(
                $"{i + 1}. {operations[i].Description} (ID: {operations[i].Id}, Сумма: {operations[i].Amount})");
        }

        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > operations.Count)
        {
            Console.WriteLine("Некорректный выбор. Повторите:");
        }

        return operations[choice - 1];
    }

    private void Pause()
    {
        Console.WriteLine("Нажмите любую клавишу для возврата в меню...");
        Console.ReadKey();
    }

    private void Exit()
    {
        Environment.Exit(0);
    }
}