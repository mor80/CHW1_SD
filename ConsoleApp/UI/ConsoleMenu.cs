namespace ConsoleApp.UI;

public class ConsoleMenu
{
    private readonly List<MenuCell> _menuCells;
    private int _selectedIndex;

    public ConsoleMenu(List<MenuCell> menuCells)
    {
        _menuCells = menuCells;
        _selectedIndex = 0;
    }

    public void Show()
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.CursorVisible = false;
            Console.WriteLine("Используйте стрелки для перемещения, Enter для выбора, Escape для выхода.\n");

            for (int i = 0; i < _menuCells.Count; i++)
            {
                if (i == _selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine(_menuCells[i].Title);
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(_menuCells[i].Title);
                }
            }

            var keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    _selectedIndex = (_selectedIndex == 0) ? _menuCells.Count - 1 : _selectedIndex - 1;
                    break;
                case ConsoleKey.DownArrow:
                    _selectedIndex = (_selectedIndex + 1) % _menuCells.Count;
                    break;
                case ConsoleKey.Enter:
                    Console.CursorVisible = true;
                    _menuCells[_selectedIndex].Action?.Invoke();
                    break;
                case ConsoleKey.Escape:
                    exit = true;
                    break;
            }
        }
    }
}