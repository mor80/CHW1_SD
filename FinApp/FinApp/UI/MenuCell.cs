namespace ConsoleApp.UI;

public class MenuCell
{
    public string Title { get; set; }
    public Action Action { get; set; }

    public MenuCell(string title, Action action)
    {
        Title = title;
        Action = action;
    }
}