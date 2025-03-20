using ClassLibrary.Entities;
using FinApp.Commands.Abstractions;
using FinApp.Facades.Abstractions;

namespace FinApp.Commands;

public class CreateOperationCommand : ICommand
{
    private readonly IFinanceFacade _facade;
    private readonly OperationType _type;
    private readonly Guid _bankAccountId;
    private readonly decimal _amount;
    private readonly DateTime _date;
    private readonly string _description;
    private readonly Guid _categoryId;

    public CreateOperationCommand(IFinanceFacade facade,
        OperationType type,
        Guid bankAccountId,
        decimal amount,
        DateTime date,
        string description,
        Guid categoryId)
    {
        _facade = facade;
        _type = type;
        _bankAccountId = bankAccountId;
        _amount = amount;
        _date = date;
        _description = description;
        _categoryId = categoryId;
    }

    public void Execute()
    {
        _facade.CreateOperation(_type, _bankAccountId, _amount, _date, _description, _categoryId);
    }
}